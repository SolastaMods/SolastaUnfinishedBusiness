# encoding: utf-8
#
# AUTHOR: magicskysword
#
# DESCRIPTION: This script is used to synchronize content from official translation to unofficial translation
#
# REQUIRES:
#   - Python 3.9.x

import os
import io
import codecs
import sys

addTermCount = 0
changeTermCount = 0
unuseTermCount = 0

def msg(text):
    print(text)

def unpack_record(record):
    term = ""
    text = ""
    try:
        (term, text) = record.split("=", 1)
        text = text.strip()
    except:
        term = record

    return term, text

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
        msg("ERROR")

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
    msg(f"sync {file_full_name}")

    global addTermCount
    global changeTermCount
    global unuseTermCount

    # sync file with offcial dict
    unused_keys = []
    for key, value in file_record.items():
        if key not in offcial_dict:
            msg(f"\t-Unused {file_full_name} {key} {value}")
            unused_keys.append(key)
            unuseTermCount += 1

    for key in unused_keys:
        del file_record[key]

    for key, value in offcial_dict.items():
        if key not in file_record or file_record[key] == "EMPTY":
            msg(f"\t+Add {file_full_name} {key} {value}")
            file_record[key] = offcial_dict[key]
            addTermCount += 1

    # check changes
    for key, value in file_record.items():
        if key in offcial_dict and file_record[key] != offcial_dict[key]:
            msg(f"\t!FindChange {file_full_name} {key}\n\t\t{offcial_dict[key]} \n\t\t-> \n\t\t{value}")
            changeTermCount += 1

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

def sync_folder(dict_group, unofficial_file_code):
    unoffcial_folder_name = f"SolastaUnfinishedBusiness\\UnofficialTranslations\\{unofficial_file_code}"
    for group_name in dict_group.keys():
        file_full_name = os.path.join(unoffcial_folder_name, f"{group_name}-{unofficial_file_code}.txt")
        if os.path.exists(file_full_name):
            file_record = readRecord(file_full_name)
            msg(f"read {file_full_name}")
        else:
            file_record = {}
            msg(f"create {file_full_name}")
        sync_file(dict_group[group_name], file_record, file_full_name)

    for file_name in os.listdir(unoffcial_folder_name):
        file_full_name = os.path.join(unoffcial_folder_name, file_name)
        group_name = file_name.split("-")[0]
        if group_name not in dict_group and file_name.endswith(".txt"):
            msg(f"unuse file {file_full_name}")

def sync_translation(offcial_file_code, unofficial_file_code):
    # read offcial translation file
    offcial_file_name = f"Diagnostics\\OfficialTranslations-{offcial_file_code}.txt"
    offcial_dict = readRecord(offcial_file_name)
    dict_group = split_dict(offcial_dict)
    # read unofficial translation group
    sync_folder(dict_group, unofficial_file_code)

def main():
    # run this script in root folder
    # sync cn language

    # if arguements contains "log", redirect stdout to log file
    if len(sys.argv) > 1 and "-log" in sys.argv:
        sys.stdout = open("..\\translateUnoffcialAutoSync.log", "w", encoding="utf-8")
        sys.stderr = sys.stdout

    sync_translation("cn-ZN", "zh-CN-Unofficial")
    msg(f"addTermCount {addTermCount}")
    msg(f"changeTermCount {changeTermCount}")
    msg(f"unuseTermCount {unuseTermCount}")


if __name__ == "__main__":
    main()