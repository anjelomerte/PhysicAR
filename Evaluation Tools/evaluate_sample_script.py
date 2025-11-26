# -*- coding: utf-8 -*-

import pandas as pd
import numpy as np
import tkinter as tk
from tkinter import filedialog
import sys

# Init tkinter but hide root window (for now)
root = tk.Tk()
root.withdraw()

# Let user choose .csv file
file_path = filedialog.askopenfilename(title="Bitte wählen sie die zu analysierende CSV-Datei aus", filetypes=[("CSV Files", "*.csv")])
# Make sure only reading file if user provided one
if file_path:
    df = pd.read_csv(file_path)
else:
    print("Es wurde keine Datei ausgewählt. Programm wird beendet.")
    
    # End program
    sys.exit(1)

# Compute total time (including idle time)
total_time = df["Timestamp"].max() - df["Timestamp"].min()

# Compute dt (time between samples) for each sample
df["dt"] = df["Timestamp"].diff().fillna(0)

# Time spent per actual area (exclude areaID 0 which indicates idle)
time_per_area = (
    df[df["AreaID"] != 0]
    .groupby("AreaID")["dt"]
    .sum()
    .reset_index()
    .rename(columns={"dt": "Zeit (s)"})
)

# Compute distances travelled by cleaner
df["CleanerPos"] = list(zip(df["CleanerX"], df["CleanerY"], df["CleanerZ"]))    # Zip positional values to thruple
distances = [0.0]                                                               # Set first distance value to 0
for i in range(1, len(df)):                                                     # Compute distances between samples
    prev = np.array(df["CleanerPos"][i-1])
    curr = np.array(df["CleanerPos"][i])
    distances.append(np.linalg.norm(curr - prev))
df["dist"] = distances

# Total distance 
total_distance = df["dist"].sum()

# Distance per area
distance_per_area = (
    df[df["AreaID"] != 0]
    .groupby("AreaID")["dist"]
    .sum()
    .reset_index()
    .rename(columns={"dist": "Distanz (m)"})
)

# Merge data frames for display
per_area_stats = pd.merge(
    time_per_area,
    distance_per_area,
    on="AreaID"
) 
per_area_stats.rename(columns={"AreaID": "Flächen ID"}, inplace=True)

# Display results
print(f"Zeit insgesamt (s): {total_time:.2f}")
print(f"Distance insgesamt (m): {total_distance:.2f}\n")
print(per_area_stats)