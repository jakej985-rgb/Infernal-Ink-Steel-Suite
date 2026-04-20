
import os

filepath = 'InfernalInkSteelSuite.Web/wwwroot/css/site.css'

with open(filepath, 'r') as f:
    content = f.read()

navbar_block_start = """/* Navbar */
.bg-glass {
  background-color: rgba(10, 10, 10, 0.95) !important;"""

if navbar_block_start in content:
    print("Navbar block start found.")
else:
    print("Navbar block start NOT found.")
    # Print the area around where it should be
    start_index = content.find("/* Navbar */")
    if start_index != -1:
        print("Found '/* Navbar */' at index", start_index)
        print("Context:", repr(content[start_index:start_index+100]))
    else:
        print("'/* Navbar */' not found.")

emergency_block_start = "/* --- EMERGENCY NAVBAR FIXES --- */"
if emergency_block_start in content:
    print("Emergency block start found.")
else:
    print("Emergency block start NOT found.")
