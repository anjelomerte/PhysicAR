import pandas as pd
import numpy as np
import sys
import tkinter as tk
from tkinter import filedialog
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
from matplotlib.animation import FuncAnimation
from matplotlib.colors import ListedColormap
from matplotlib.widgets import Slider
from matplotlib.transforms import Affine2D

# ── KONFIGURATION ─────────────────────────────────────────────────────────────
TILES_X = 3
TILES_Z = 4
CLEANER_W = 0.173
CLEANER_H = 0.247
STEP = 1
INTERPOLATION_DIST = 0.10 

SEQUENCE = [4, 12, 10, 1, 11, 6, 7, 9, 2, 8, 3, 5, 7, 12, 10, 1, 11, 6, 4, 9, 2, 8, 3, 5, 7, 12, 1, 11, 3, 4]

def get_field_coords(num):
    row = (num - 1) // 3  
    col = (num - 1) % 3   
    return col, (3 - row)

# ──────────────────────────────────────────────────────────────────────────────

root = tk.Tk()
root.withdraw()
input_path = filedialog.askopenfilename(title="CSV auswählen")
if not input_path:
    sys.exit(0)

df = pd.read_csv(input_path)
df.columns = df.columns.str.strip().str.lstrip('\ufeff')

# Geometrie
ptl = np.array([df['TopLeftX'].iloc[0], df['TopLeftZ'].iloc[0]])
pbr = np.array([df['BottomRightX'].iloc[0], df['BottomRightZ'].iloc[0]])
diag_v = pbr - ptl
L = np.linalg.norm(diag_v)
FIELD_W, FIELD_H = (3/5) * L, (4/5) * L
alpha = np.arctan2(diag_v[1], diag_v[0])
phi = np.arctan2(FIELD_H, FIELD_W) 
final_theta = (alpha - phi)

def to_local(xw, zw):
    dx, dz = xw - ptl[0], zw - ptl[1]
    xl = dx * np.cos(final_theta) + dz * np.sin(final_theta)
    zl = -dx * np.sin(final_theta) + dz * np.cos(final_theta)
    return xl, zl

loc_x, loc_z = to_local(df['CleanerX'].values, df['CleanerZ'].values)
CELL_W, CELL_H = FIELD_W / (TILES_X * 10), FIELD_H / (TILES_Z * 10)

# ── STATE ─────────────────────────────────────────────────────────────────────
state = {
    'idx': 0,
    'paused': False,
    'field_state': {
        i: {
            "start_time": None,
            "end_time": None,
            "tiles_cleaned": 0
        }
        for i in range(len(SEQUENCE))
    }
}

field_log = []
last_end_time = None
global_start_time = df['Timestamp'].iloc[0]
current_time = 0

# ── CLEAN LOGIC ────────────────────────────────────────────────
def clean_at_pos_touch_side(cx, cz, grid, f_col, f_row):
    rel_x = (cx - (f_col * FIELD_W/3))
    rel_z = (cz - (f_row * FIELD_H/4))

    c_min = max(0, int(np.floor((rel_x - CLEANER_H/2) / CELL_H)))
    c_max = min(9, int(np.floor((rel_x + CLEANER_H/2) / CELL_H)))

    r_min = max(0, int(np.floor((rel_z - CLEANER_W/2) / CELL_W)))
    r_max = min(9, int(np.floor((rel_z + CLEANER_W/2) / CELL_W)))

    for r in range(r_min, r_max + 1):
        for c in range(c_min, c_max + 1):
            if grid[r, c]:
                if current_seq_idx < len(SEQUENCE):
                    fs = state['field_state'][current_seq_idx]

                    # Start erst bei tatsächlich gereinigtem Tile
                    if fs["start_time"] is None:
                        fs["start_time"] = current_time

                grid[r, c] = False

                if current_seq_idx < len(SEQUENCE):
                    state['field_state'][current_seq_idx]["tiles_cleaned"] += 1


def clean_at_pos_touch_center(cx, cz, grid, f_col, f_row):
    c_left = cx - CLEANER_H/2
    c_right = cx + CLEANER_H/2
    c_bottom = cz - CLEANER_W/2
    c_top = cz + CLEANER_W/2

    f_origin_x = f_col * FIELD_W/3
    f_origin_z = f_row * FIELD_H/4

    c_start = max(0, int((c_left - f_origin_x) / CELL_H))
    c_end = min(9, int((c_right - f_origin_x) / CELL_H))
    r_start = max(0, int((c_bottom - f_origin_z) / CELL_W))
    r_end = min(9, int((c_top - f_origin_z) / CELL_W))

    for r in range(r_start, r_end + 1):
        for c in range(c_start, c_end + 1):
            if grid[r, c]:
                tile_center_x = f_origin_x + (c + 0.5) * CELL_H
                tile_center_z = f_origin_z + (r + 0.5) * CELL_W

                if (c_left <= tile_center_x <= c_right and 
                    c_bottom <= tile_center_z <= c_top):
                    grid[r, c] = False


def clean_at_pos_cover(cx, cz, grid, f_col, f_row):
    MARGIN = 0.02 
    c_left = cx - CLEANER_W/2 - MARGIN
    c_right = cx + CLEANER_W/2 + MARGIN
    c_bottom = cz - CLEANER_H/2 - MARGIN
    c_top = cz + CLEANER_H/2 + MARGIN

    f_origin_x = f_col * FIELD_W/3
    f_origin_z = f_row * FIELD_H/4

    c_start = max(0, int((c_left - f_origin_x) / CELL_W))
    c_end = min(9, int((c_right - f_origin_x) / CELL_W))
    r_start = max(0, int((c_bottom - f_origin_z) / CELL_H))
    r_end = min(9, int((c_top - f_origin_z) / CELL_H))

    for r in range(r_start, r_end + 1):
        for c in range(c_start, c_end + 1):
            if grid[r, c]:
                tile_l = f_origin_x + c * CELL_W
                tile_r = f_origin_x + (c + 1) * CELL_W
                tile_b = f_origin_z + r * CELL_H
                tile_t = f_origin_z + (r + 1) * CELL_H

                if (tile_l >= c_left and tile_r <= c_right and 
                    tile_b >= c_bottom and tile_t <= c_top):
                    grid[r, c] = False

# ── SIMULATION ───────────────────────────────────────────────────────────────
print("Simuliere Echtzeit-Daten...")

simulation_frames = []
all_times = []
current_seq_idx = 0
field_grids = [np.ones((10, 10), dtype=bool) for _ in SEQUENCE]
last_pos = None

for i in range(0, len(df), STEP):
    if current_seq_idx >= len(SEQUENCE):
        break

    t = df['Timestamp'].iloc[i]
    current_time = t
    curr_pos = np.array([loc_x[i], loc_z[i]])

    # interpolation
    if last_pos is not None:
        dist = np.linalg.norm(curr_pos - last_pos)
        if dist > INTERPOLATION_DIST:
            num_steps = int(dist / INTERPOLATION_DIST)
            for s in range(1, num_steps):
                interp_pos = last_pos + (curr_pos - last_pos) * (s / num_steps)
                f_col, f_row = get_field_coords(SEQUENCE[current_seq_idx])
                clean_at_pos_touch_side(interp_pos[0], interp_pos[1],
                                         field_grids[current_seq_idx],
                                         f_col, f_row)

    if current_seq_idx < len(SEQUENCE):
        fs = state['field_state'][current_seq_idx]
        start_time = fs["start_time"]
        tiles = fs["tiles_cleaned"]

        f_col, f_row = get_field_coords(SEQUENCE[current_seq_idx])
        clean_at_pos_touch_side(curr_pos[0], curr_pos[1],
                                field_grids[current_seq_idx],
                                f_col, f_row)

        if not np.any(field_grids[current_seq_idx]):
            end_time = t

            start_time = fs["start_time"]
            tiles = fs["tiles_cleaned"]

            idle_before = (
                start_time - global_start_time
                if current_seq_idx == 0
                else start_time - last_end_time
            )

            field_log.append({
                "field_id": current_seq_idx,
                "start_time": round(start_time, 3),
                "end_time": round(end_time, 3),
                "duration": round(end_time - start_time, 3),
                "tiles_cleaned": tiles,
                "progress": round(tiles / 100.0, 2),
                "idle_before": round(idle_before, 3)
            })

            last_end_time = end_time

            current_seq_idx += 1

    if current_seq_idx < len(SEQUENCE):
        display_grid = np.zeros((40, 30), dtype=float)

        active_f_num = SEQUENCE[current_seq_idx]
        ac_col, ac_row = get_field_coords(active_f_num)

        display_grid[ac_row*10:(ac_row+1)*10,
                     ac_col*10:(ac_col+1)*10] = field_grids[current_seq_idx]

        active_tiles = state['field_state'][current_seq_idx]["tiles_cleaned"]

        total_tiles = sum(
            fs["tiles_cleaned"]
            for fs in state['field_state'].values()
        )

        simulation_frames.append((
            t,
            display_grid,
            curr_pos[0],
            curr_pos[1],
            current_seq_idx,
            active_tiles,
            total_tiles
        ))
        all_times.append(t)

    last_pos = curr_pos

all_times = np.array(all_times)

# ── PLOT + ANIMATION (unverändert) ───────────────────────────────────────────
fig, ax = plt.subplots(figsize=(8, 10))
plt.subplots_adjust(bottom=0.15)

cmap = ListedColormap(['#fdfdfd', '#4CAF50'])

im = ax.imshow(simulation_frames[0][1],
               origin='lower',
               extent=[0, FIELD_W, 0, FIELD_H],
               cmap=cmap,
               vmin=0,
               vmax=1,
               interpolation='nearest')

path_line, = ax.plot([], [], color='#FF5722', lw=1, alpha=0.4)

cleaner_rect = mpatches.Rectangle((-CLEANER_W/2, -CLEANER_H/2),
                                  CLEANER_W, CLEANER_H,
                                  ec='#D32F2F',
                                  fc='#FF572222',
                                  lw=2)
ax.add_patch(cleaner_rect)

ax.set_xticks([])
ax.set_yticks([])
ax.set_aspect('equal')

stats_text = ax.text(
    0.02,
    0.98,
    "",
    transform=ax.transAxes,
    va='top',
    ha='left',
    fontsize=10,
    family='monospace',
    bbox=dict(
        facecolor='white',
        alpha=0.8,
        edgecolor='none'
    )
)

ax_slider = plt.axes([0.2, 0.05, 0.6, 0.03])
slider = Slider(ax_slider, 'Zeit',
                all_times[0], all_times[-1],
                valinit=all_times[0],
                valfmt='%.1f s')

smoothed_angle = 0.0

def set_slider_silent(val):
    slider.eventson = False
    slider.set_val(val)
    slider.eventson = True

def render_frame(idx, sync_slider=False):
    global smoothed_angle

    state['idx'] = idx

    t, grid, cx, cz, field_idx, active_tiles, total_tiles = simulation_frames[idx]
    im.set_data(grid)

    sim_x = np.array([f[2] for f in simulation_frames])
    sim_z = np.array([f[3] for f in simulation_frames])
    path_line.set_data(sim_x[:idx], sim_z[:idx])

    window = 5
    start = max(0, idx - window)

    dx = loc_x[idx] - loc_x[start]
    dz = loc_z[idx] - loc_z[start]

    dist = np.hypot(dx, dz)

    if dist > 0.005:
        target_angle = np.degrees(np.arctan2(dz, dx))
        diff = (target_angle - smoothed_angle + 180) % 360 - 180
        smoothed_angle += 0.15 * diff

    transform = Affine2D().rotate_deg(smoothed_angle).translate(cx, cz) + ax.transData
    cleaner_rect.set_transform(transform)

    progress = active_tiles / 100.0

    field_start_time = field_log[field_idx]["start_time"] if field_idx < len(field_log) else t

    field_time = max(0, t - field_start_time)

    stats_text.set_text(
        f"Completed fields: {field_idx}\n"
        f"Tiles: {active_tiles}/100\n"
        f"Field Progress: {progress:.0%}\n"
        f"Field Time: {field_time:.2f}s\n"
    )

    if sync_slider:
        set_slider_silent(all_times[idx])

    fig.canvas.draw_idle()

def update_view(val):
    idx = np.abs(all_times - val).argmin()
    render_frame(idx)

slider.on_changed(update_view)

def anim_step(_):
    if state['paused']:
        return

    new_idx = (state['idx'] + 1) % len(all_times)
    render_frame(new_idx, sync_slider=True)

def on_key(event):
    if event.key == ' ':
        state['paused'] = not state['paused']

    elif event.key == 'right':
        idx = min(len(all_times)-1, state['idx'] + 1)
        render_frame(idx, sync_slider=True)

    elif event.key == 'left':
        idx = max(0, state['idx'] - 1)
        render_frame(idx, sync_slider=True)

fig.canvas.mpl_connect('key_press_event', on_key)

ani = FuncAnimation(fig, anim_step, interval=30, blit=False, cache_frame_data=False)

plt.show()

# ── EXPORT ───────────────────────────────────────────────────────────────────
pd.DataFrame(field_log).to_csv("field_summary.csv", index=False)