#
# AUTHOR: Zappastuff - 2021-APR
#
# REQUIRES:
#   - Python 3.9.x
#   - deep_translator library (pip3 install deep_translator)
#

import argparse
import os
import re
import shutil
import sys
from deep_translator import GoogleTranslator


OUTPUT_FOLDER = "Translations-"
CHARS_MAX = 5000
SEPARATOR = "\x0D"


def parse_command_line():
    my_parser = argparse.ArgumentParser(description='Translates Solasta game terms')
    my_parser.add_argument('input_file',
                        type=str,
                        help='input file')
    my_parser.add_argument('output_file',
                        type=str,
                        help='output file')
    my_parser.add_argument('-c', '--code',
                        type=str,
                        required=True,
                        help='language code')
    my_parser.add_argument('-d', '--dict',
                        type=str,
                        help='dictionary file')


    return my_parser.parse_args()


def load_dictionary(filename):
    dictionary = {}
    if not filename:
       pass
    elif not os.path.exists(filename):
        print(f"WARNING: dictionary file doesn't exist. using an empty one")
    else:
        with open(filename, "rt", encoding="utf-8") as f:
            record = "\n"
            while record:
                record = f.readline()
                if record and record.split():
                    try:
                        (f, r) = record.split(" ", 1).strip()
                        dictionary[f] = r
                    except:
                        print(f"ERROR: skipping dictionary line {record}")

    return dictionary


def display_progress(count, total, status=''):
    bar_len = 80
    filled_len = int(round(bar_len * count / float(total)))

    percents = round(100.0 * count / float(total), 1)
    bar = '=' * filled_len + '-' * (bar_len - filled_len)

    sys.stdout.write('[%s] %s%s (%s)\r' % (bar, percents, '%', status))
    sys.stdout.flush() 


def unpack_record(record):
    term = ""
    text = ""
    try:
        (term, text) = record.split("=", 1)
        text = text.strip()
    except:
        term = record

    return term, text if text != "" else "EMPTY"


def get_records(filename):
    try:
        line_count = 0
        line_total = sum(1 for line in open(filename))
        with open(filename, "rt", encoding="utf-8") as f:
            record = "\n"
            while record:
                display_progress(line_count, line_total, filename)
                record = f.readline()
                line_count += 1
                if record: yield unpack_record(record)
            print()

    except FileNotFoundError:
        print("ERROR")


def translate_chunk(text, code):
    text = text.replace("\\n", "\n")
    translated = GoogleTranslator(source="auto", target=code).translate(text) if len(text) <= CHARS_MAX else text
    translated = translated.replace("\n", "\\n")

    return translated


# kiddos: this is ugly ;-)
r0 = re.compile(r"<# ([A-F0-9]*?)>")
r1 = re.compile(r"<#([A-F0-9]*?)> (.*?) </color>")
r2 = re.compile(r"<i> (.*?) </i>")
r3 = re.compile(r"<b> (.*?) </b>")

def fix_translated_format(text):
    text = r0.sub(r"<#\1>", text)
    text = r1.sub(r"<#\1>\2</color>", text)
    text = r2.sub(r"<i>\1</i>", text)
    text = r3.sub(r"<b>\1</b>", text) 

    return text


def apply_dictionary(dictionary, text):
    # text = text.replace("</color> ", "</color>")

    for key in dictionary:
        text = text.replace(key, dictionary[key])

    return text


def translate_file(input_file, output_file, code, dictionary=None):
    with open(output_file, "wt", encoding="utf-8") as f:

        for term, text in get_records(input_file):
            translated = translate_chunk(text, code)
            fixed = fix_translated_format(translated)
            replaced = apply_dictionary(dictionary, fixed)
            f.write(f"{term}={replaced}\n")


def main():
    args = parse_command_line()
    translate_file(
        args.input_file,
        args.output_file,
        args.code,
        load_dictionary(args.dict))

if __name__ == "__main__":
    main()