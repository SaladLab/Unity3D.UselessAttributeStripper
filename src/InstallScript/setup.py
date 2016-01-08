#!/usr/local/bin/python

import os
import sys
import shutil


def find_stipper_path(unity_path):
    if os.name == "nt":
        # Windows (C:\Program Files\Unity\Editor\Data\Tools\UnusedByteCodeStripper2)
        base_path = unity_path or r"C:\Program Files\Unity"
        stripper_path = os.path.join(base_path, r"Editor\Data\Tools\UnusedByteCodeStripper2")
    else:
        # OSX (/Applications/Unity/Unity.app/Contents/Frameworks/Tools/UnusedByteCodeStripper2)
        print "nt"    
        base_path = unity_path or r"/Applications/Unity"
        stripper_path = os.path.join(base_path, r"Unity.app/Contents/Frameworks/Tools/UnusedByteCodeStripper2")

    if os.path.exists(stripper_path):
        print "Path:", stripper_path
        return stripper_path
    else:
        print "ERROR: Cannot find UnusedByteCodeStripper2 directory"
        return None


def install(unity_path):
    target_path = find_stipper_path(unity_path)
    if not target_path:
        return 1

    if os.path.exists(os.path.join(target_path, "UnusedBytecodeStripper2.org.exe")):
        print "ERROR: UselessAttributeStripper is already installed."
        return 1

    shutil.move(os.path.join(target_path, "UnusedBytecodeStripper2.exe"), 
                os.path.join(target_path, "UnusedBytecodeStripper2.org.exe"))
    shutil.move(os.path.join(target_path, "UnusedBytecodeStripper2.exe.mdb"), 
                os.path.join(target_path, "UnusedBytecodeStripper2.org.exe.mdb"))
    shutil.copy("UselessAttributeStripper.exe",
                os.path.join(target_path, "UnusedBytecodeStripper2.exe"))
    if os.name == "posix":
        os.chmod(os.path.join(target_path, "UnusedBytecodeStripper2.exe"), 0755)

    print "done!"
    return 0


def uninstall(unity_path):
    target_path = find_stipper_path(unity_path)
    if not target_path:
        return 1

    if os.path.exists(os.path.join(target_path, "UnusedBytecodeStripper2.org.exe")) == False:
        print "ERROR: UselessAttributeStripper is not installed."
        return 1

    os.remove(os.path.join(target_path, "UnusedBytecodeStripper2.exe"))
    shutil.move(os.path.join(target_path, "UnusedBytecodeStripper2.org.exe"), 
                os.path.join(target_path, "UnusedBytecodeStripper2.exe"))
    shutil.move(os.path.join(target_path, "UnusedBytecodeStripper2.org.exe.mdb"), 
                os.path.join(target_path, "UnusedBytecodeStripper2.exe.mdb"))

    print "done!"
    return 0


def check(unity_path):
    target_path = find_stipper_path(unity_path)
    if not target_path:
        return 1

    if os.path.exists(os.path.join(target_path, "UnusedBytecodeStripper2.org.exe")):
        print "UselessAttributeStripper is installed."
    else:
        print "UselessAttributeStripper is not installed."

    return 0


def show_usage():
    print "setup.py command [unity-path]"
    print "command:"
    print "   install     install UselessAttributeStripper to unity"
    print "   uninstall   uninstall UselessAttributeStripper from unity"
    print "   check       check if UselessAttributeStripper is installed or not"


def main():
    command = sys.argv[1] if len(sys.argv) > 1 else ""
    unity_path = sys.argv[2] if len(sys.argv) > 2 else ""
    if command == "install":
        sys.exit(install(unity_path))
    elif command == "uninstall":
        sys.exit(uninstall(unity_path))
    elif command == "check":
        sys.exit(check(unity_path))
    else:
        show_usage()
        sys.exit(1)


if __name__ == '__main__':
    main()
