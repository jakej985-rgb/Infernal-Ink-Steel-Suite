
import os

filepath = 'InfernalInkSteelSuite.Web/wwwroot/css/site.css'

with open(filepath, 'r') as f:
    content = f.read()

# Check for Dropdowns section
if "/* Dropdowns */" in content:
    print("Dropdowns section found.")
else:
    print("Dropdowns section NOT found.")

# Check for nav-link update
if ".nav-link" in content:
    idx = content.find(".nav-link {")
    if idx != -1:
        snippet = content[idx:idx+200]
        print("Nav-link snippet:", repr(snippet))
    else:
        print("nav-link rule not found.")

# Check for Emergency block
if "/* --- EMERGENCY NAVBAR FIXES --- */" in content:
    print("Emergency block found.")
else:
    print("Emergency block NOT found.")
