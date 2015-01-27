	

    #include <stdio.h>
    #include <windows.h>
    #include <iostream>
    #include <time.h>
    #include <cstdlib>
    #include <string>
    #include <math.h>
    #include <conio.h>
    #include <signal.h>
    #include <stdlib.h>
    #include <cstdio>
    #include <tlhelp32.h>
    #include <sys/stat.h>
    #include <cctype>
    #include <cstdarg>
    #include <vector>
    #include <algorithm>
    // :^)
     
    using namespace std;
    const char* LoLProcess = "League of Legends.exe";
    bool pressedf5 = false;
    bool isAlive(const char* pN);
    void emulatef5();
     
    int main(char* envp[])
    {
            cout << "quick simple yet soplusplus auto f5 tool, now with 120 percent more memory leak!" << endl;
            cout << endl;
            while (1)
            {
					Sleep(5000);
                    if (isAlive(LoLProcess) && !pressedf5)
                    {
                            Sleep(30000); //I think we have to wait for game to load? idk, change as needed.
                            cout << "Pressed F5 :)" << endl;
                            emulatef5();
                            pressedf5 = true;
                    }
                    if (!isAlive(LoLProcess))
                    {
                            pressedf5 = false;
                    }
					cout << "Waiting for new LoL instance" << endl;
     
            }
    }
     
    void emulatef5()
    {
            INPUT ip;
            ip.type = INPUT_KEYBOARD;
            ip.ki.wScan = 0;
            ip.ki.time = 0;
            ip.ki.dwExtraInfo = 0;
     
            // Press F5
            ip.ki.wVk = VK_F5;
            ip.ki.dwFlags = 0;
            SendInput(1, &ip, sizeof(INPUT));
     
            // Release F5
            ip.ki.wVk = VK_F5;
            ip.ki.dwFlags = KEYEVENTF_KEYUP;
            SendInput(1, &ip, sizeof(INPUT));
    }
    bool isAlive(const char* pN)
    {
            HANDLE SnapShot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
     
            if (SnapShot == INVALID_HANDLE_VALUE)
			{
					delete SnapShot;
                    return false;
			}
     
            PROCESSENTRY32 procEntry;
            procEntry.dwSize = sizeof(PROCESSENTRY32);
     
            if (!Process32First(SnapShot, &procEntry))
			{
					delete SnapShot;
                    return false;
			}

            do
            {
                    if (strcmp(procEntry.szExeFile, pN) == 0)
					{
						return true;
					}
            } while (Process32Next(SnapShot, &procEntry));
            return false;
    }

