import os
import re

def unpack_record(record):
    term = ""
    text = ""
    try:
        (term, text) = record.split("=", 1)
        text = text.strip()
    except:
        term = record

    return term, text if text != "" else "EMPTY"

def upper_repl(match):
     return match.group(1) + "=" + match.group(2).upper() + match.group(3)

for root, dirs, files in os.walk('.'):
    for file in files:
        if file.endswith('.txt'):
            filename = os.path.join(root, file)
            print(f"sorting {filename}")
            with open(filename, "rt", encoding="utf-8") as f:
                data = f.readlines()
                data[0] = data[0].replace('﻿', '')
                for idx, record in enumerate(data):
                    data[idx] = re.sub(r"(.+?)=(.)(.*)", upper_repl, data[idx])
                data.sort()
            with open(filename, "wt", encoding="utf-8") as f:
                f.writelines(data)