#
# AUTHOR: magicskysword
#
# DESCRIPTION: This script is used to synchronize content from official translation to unofficial translation
#
# REQUIRES:
#   - Python 3.9.x

import os
import codecs
from deep_translator import GoogleTranslator

CHARS_MAX = 4500

def unpack_record(record):
    term = ""
    text = ""
    try:
        (term, text) = record.split("=", 1)
        text = text.strip()
    except:
        term = record

    return term, text if text != "" else "EMPTY"

def translate_text(text, code):
    text = text.replace("\\n", "{99}")
    if len(text) > 3 and len(text) <= CHARS_MAX:
        translated = GoogleTranslator(source="auto", target=code).translate(text)
    else:
        translated = text
    translated = translated.replace("{99}", "\\n")

    return translated

def readRecord(filename):
    # read file and split with "=" to dict
    dic = {}
    try:
        line_count = 0
        with open(filename, "rt", encoding="utf-8") as f:
            record = "\n"
            while record:
                record = f.readline()
                # remove BOM
                if line_count == 0 and record.startswith(codecs.BOM_UTF8.decode("utf-8")):
                    record = record[1:]
                line_count += 1
                if record:
                    term, text = unpack_record(record)
                    dic[term] = text
    except FileNotFoundError:
        print("ERROR")

    return dic

def split_dict(dict):
    # split dict by /& in key
    dict_group = {}
    for key, value in dict.items():
        key_group = "Other"
        if "/&" in key:
            key_group = key.split("/&")[0]
        if key_group not in dict_group:
            dict_group[key_group] = {}
        dict_group[key_group][key] = value
    return dict_group

def sync_file(offcial_dict, file_record, file_full_name):
    # sync file with offcial dict
    for key, value in file_record.items():
        if key not in offcial_dict:
            print(f"unused {file_full_name} {key} {value}")

    for key, value in offcial_dict.items():
        if key not in file_record:
            print(f"Add {file_full_name} {key} {value}")
            file_record[key] = offcial_dict[key]

    # write file
    with open(file_full_name, "wt", encoding="utf-8") as f:
        for key, value in file_record.items():
            f.write(f"{key}={value}\n")

    # sort file
    with open(file_full_name, "rt", encoding="utf-8") as f:
        data = f.readlines()
        data[0] = data[0].replace('﻿', '')
        data.sort()
    with open(file_full_name, "wt", encoding="utf-8") as f:
        f.writelines(data)

def sync_folder(offcial_dict, unofficial_file_code, dict_group):
    unoffcial_folder_name = f"SolastaUnfinishedBusiness\\UnofficialTranslations\\{unofficial_file_code}"
    for filename in os.listdir(unoffcial_folder_name):
        if filename.endswith(".txt"):
            file_full_name = os.path.join(unoffcial_folder_name, filename)
            group_name = os.path.splitext(filename)[0].replace(unofficial_file_code, "")
            if group_name in dict_group:
                file_record = readRecord(file_full_name)
                sync_file(offcial_dict[group_name], file_record, file_full_name)



def sync_translation(offcial_file_code, unofficial_file_code):
    # read offcial translation file
    offcial_file_name = f"Diagnostics\\OfficialTranslations-{offcial_file_code}.txt"
    offcial_dict = readRecord(offcial_file_name)
    dict_group = split_dict(offcial_dict)
    # read unofficial translation group
    sync_folder(offcial_dict, unofficial_file_code, dict_group)

def main():
    # sync cn language
    sync_translation("cn-ZN", "zh-CN-Unoffcial")


if __name__ == "__main__":
    main()