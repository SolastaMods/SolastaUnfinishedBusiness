import os

def sort_files_in_folder(folder_name):
    for filename in os.listdir(folder_name):
        input_file = os.path.join(folder_name, filename)
        if os.path.isfile(input_file):                
            with open(input_file, "rt", encoding="utf-8") as f:  
                data = f.readlines()
            data.sort()
            with open(input_file, "wt", encoding="utf-8") as f:
                f.writelines(data)                               
        else:
            sort_files_in_folder(input_file)

def main():
    sort_files_in_folder("./Translations")

if __name__ == "__main__":
    main()
