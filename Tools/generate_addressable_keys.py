import os
import re
from collections import defaultdict

GAME_RES = os.path.join('Assets','GameRes')
OUTPUT = os.path.join('Assets','HotUpdate','Key','AddressableKeys.cs')
AUTO_HEADER = "// 此文件由 GameResAssetProcessor 自动生成\n// 请勿手动修改此文件\n"

def sanitize_path_segment(name):
    if not name:
        return 'Empty'
    result = re.sub(r'[^a-zA-Z0-9_]', '_', name)
    if not result.strip('_'):
        return 'Empty'
    return result.lstrip('_')

def sanitize_class_name(name):
    if not name:
        return 'Empty'
    result = re.sub(r'[^a-zA-Z0-9_]', '_', name)
    if not result.strip('_'):
        return 'Empty'
    if result[0].isdigit():
        result = '_' + result
    return result

def normalize_extension(ext):
    trimmed = ext.lstrip('.')
    if not trimmed:
        return 'File'
    s = sanitize_class_name(trimmed)
    return s[0].upper() + s[1:]

# collect files (exclude directories and .meta)
all_files = []
for root, dirs, files in os.walk(GAME_RES):
    for f in files:
        if f.endswith('.meta'):
            continue
        full = os.path.join(root, f).replace('\\', '/')
        all_files.append(full)

all_files.sort()

# build directory structure
dirs_map = defaultdict(list)
for path in all_files:
    rel = path[len('Assets/GameRes/'):] if path.startswith('Assets/GameRes/') else path
    dirn = os.path.dirname(rel).replace('\\','/')
    dirs_map[dirn].append(path)

lines = []
lines.append(AUTO_HEADER)
lines.append('namespace HotUpdate.Utility')
lines.append('{')
lines.append('public static class AddressableKeys')
lines.append('{')

for path in all_files:
    rel = path[len('Assets/GameRes/'):] if path.startswith('Assets/GameRes/') else path
    file_name_no_ext = os.path.splitext(os.path.basename(rel))[0]
    ext = normalize_extension(os.path.splitext(rel)[1])
    member = f"{sanitize_path_segment(file_name_no_ext)}_{ext}"
    lines.append(f'    public const string {member} = "{path}";')

if all_files and dirs_map:
    lines.append('')

# getters
for dir_key in sorted(dirs_map.keys()):
    # determine shared extension
    exts = set([os.path.splitext(p)[1] for p in dirs_map[dir_key]])
    exts = [e for e in exts if e!='']
    shared = ''
    if len(set(exts)) == 1:
        shared = exts[0]
    getter_name = 'GetGameRes' if dir_key=='' else 'Get' + '_'.join([sanitize_path_segment(x) for x in dir_key.split('/') if x!=''])
    dirpath = 'Assets/GameRes' if dir_key=='' else f'Assets/GameRes/{dir_key}'
    shared_suffix = normalize_extension(shared) if shared else ''
    lines.append(f'    public static string {getter_name}(string path) => "{dirpath}/" + path + "{shared_suffix}";')

lines.append('}')
lines.append('}')

os.makedirs(os.path.dirname(OUTPUT), exist_ok=True)
with open(OUTPUT, 'w', encoding='utf-8') as fh:
    fh.write('\n'.join(lines))

print('Generated', OUTPUT)
