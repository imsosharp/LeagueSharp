	

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
    const char* BugsplatEXE = "BsSndRpt.exe";
	const char* RitoBotEXE = "RitoBot.exe";
	const char* BoLEXE = "BoL Studio.exe";
    bool pressedf5 = false;
    bool isAlive(const char* pN);
    void emulatef5();
     
    int main(char* envp[])
    {
            cout << "Quick fix for bugsplats.." << endl;
            cout << endl;
            while (1)
            {
					Sleep(1000);
					if (!isAlive(RitoBotEXE))
					{
						system("start RitoBot.exe");
					}
                    if (isAlive(BugsplatEXE))
                    {
                            system("taskkill /F /T /IM BsSndRpt.exe");
							system("taskkill /F /T /IM RitoBot.exe");
							if (isAlive(BoLEXE))
							{
							system("taskkill /F /T /IM BoL Studio.exe");
							}
                    }
     
            }
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

