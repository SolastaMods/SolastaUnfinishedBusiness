#
# AUTHOR: magicskysword
#
# DESCRIPTION: This script is used to synchronize the translation from other languages.
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

def sync_file(input_file, output_file, code):
    inputDict = readRecord(input_file)
    outputDict = readRecord(output_file)
    print(f"sync\t{output_file}\tfrom\t{input_file}")
    # compare
    for key, value in inputDict.items():
        if key not in outputDict:
            # outputDict[key] = value
            outputDict[key] = translate_text(value, code)
            print(f"\t+ {output_file} add：{key}={value}")
    # write
    with open(output_file, "wt", encoding="utf-8") as f:
        for key, value in outputDict.items():
            f.write(f"{key}={value}\n")
    # compare
    for key, value in outputDict.items():
        if key not in inputDict:
            print(f"\t- {output_file} needless：{key}={value}")
    # sort
    with open(output_file, "rt", encoding="utf-8") as f:
        data = f.readlines()
        data.sort()
    with open(output_file, "wt", encoding="utf-8") as f:
        f.writelines(data)

def sync_folder(root_folder_name, folder_name, code):
    root_output_name = code if code != "pt" else "pt-BR"
    for filename in os.listdir(folder_name):
        input_file = os.path.join(folder_name, filename)
        if os.path.isfile(input_file):
            output_file = f"{root_output_name}\\{input_file[3:-7]}-{root_output_name}.txt"
            sync_file(input_file, output_file, code)
        else:
            sync_folder(root_folder_name, input_file, code)


def main():
    # sync cn language
    sync_folder("en", "en", "zh-CN")
    sync_folder("en", "en", "ko")
    sync_folder("en", "en", "pt")
    sync_folder("en", "en", "es")
    sync_folder("en", "en", "fr")
    sync_folder("en", "en", "de")
    sync_folder("en", "en", "it")

if __name__ == "__main__":
    main()