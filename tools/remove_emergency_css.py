
import os

filepath = 'InfernalInkSteelSuite.Web/wwwroot/css/site.css'

with open(filepath, 'r') as f:
    content = f.read()

start_marker = "/* --- EMERGENCY NAVBAR FIXES --- */"
end_marker = "/* --- Utility Classes (Added for Top Bar Icons) --- */"

start_idx = content.find(start_marker)
end_idx = content.find(end_marker)

if start_idx != -1 and end_idx != -1:
    new_content = content[:start_idx] + content[end_idx:]
    with open(filepath, 'w') as f:
        f.write(new_content)
    print("Emergency block removed.")
else:
    print("Could not find markers to remove block.")
    print("Start found:", start_idx != -1)
    print("End found:", end_idx != -1)
