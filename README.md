
# Bitcrack-Randomiser

**Bitcrack-Randomiser** works integrated with the application called Bitcrack. 

![alt text](https://i.ibb.co/DGq2Bbz/ap.png)

## Pool Statistics

Go to https://btcpuzzle.info/

## How it works?

It only works with BTC Puzzle 66,67 and 68 (You can change the puzzle number from the [target_puzzle] variable in the settings.txt file.).

## Example Puzzle 66

If you want to **scan all private keys in  puzzle 66**; you need to do 36 quintillion scans in total. In case you do a random scan; previously generated private keys will be regenerated (random problem). This extends the scan time by x10. Puzzle 66 HEX ranges as follows. It starts with 2 or 3. Any private key in this range is **17 characters long.**

`20000000000000000 to
3ffffffffffffffff`

**We take the first 7 characters** and delete the rest for now. The result will be as follows.

`2000000 to
3ffffff`

We now have about 33 million possible private keys to search. All possible private keys are **stored in the database**. A random value will come up each time a scan job is called and **will be marked as scanned** when the scan is complete. 

I can scan each key in about 10 minutes on NVIDIA 3090. This actually means about 1,1 trillion private keys. When the private key is scanned, it is marked as scanned. So it won't show up anymore.

## Start arguments

You can use start arguments to test some events.

`dotnet BitcrackRandomiser.dll --mode-test` Run application in test mode. Example private key will be found in a few seconds for testing.

`dotnet BitcrackRandomiser.dll --mode-telegramtest` Check if the Telegram sharing function is working.

## Settings

You can update the application settings via the "settings.txt" file.

### [**target_puzzle**] `*Required*`

Select the puzzle you want to scan. Default: 66

`66` or `67` or `68`

---

### [**bitcrack_path**] `*Required*`

Add the folder where the Bitcrack application is located in the first line. **Note: Only for NVIDIA CUDA devices. Do not use clBitCrack.exe**

`C:\Users\{YOURUSERNAME}\App\bitcrack\cuBitCrack.exe` 

---

### [**bitcrack_arguments**] 

You can write the arguments for Bitcrack. For default settings leave blank.

`-b 896 -t 256 -p 256` or `-t 128` or leave blank.

Note: Do not use `-o --keyspace` parameters.

---

### [**wallet_address**] `*Required*`

Enter the BTC address here. Remember for now **this is not a pool**! But if in the future please enter a BTC recipient address.

`1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw` or `any worker name`

Recommended: Your BTC Wallet Address 

---

### [**scantype**] `*Required*`

Select a scan type.

`default` - Scan and exclude defeated ranges.

`includeDefeatedRanges` - Scan and include defeated ranges.

---

### [**custom_range**] `*Required*`

Scan custom range

`none` Scan all of ranges.

`{VALUE}` Scan for keys starting with {VALUE}.

Enter the first 2 characters of the range you want to scan. Possible values: `20,21,22,23,24,25,26,27,28,29,2A,2B,2C,2D,2E,2F,30,31,32,33,34,35,36,37,38,39,3A,3B,3C,3D,3E,3F`

Note: Valid between 2 and 5 characters.

---

### [**telegram_share**] `*Required*`

Share progress to Telegram

`true` Share progress to Telegram. 

`false` If false, it does not send notification. 

---

### [**telegram_acesstoken**] `*Required* if telegram_share=true`

`{ACCESS_TOKEN}` Telegram BOT access token

Example: 6331494066:ABEfv4cF3dbc3mA8qGLDlEp2uxzgYESIa_w

---

### [**telegram_chatid**] `*Required* if telegram_share=true`

`{CHAT_ID}` Telegram chat id

Example: -9334716240

---

### [**telegram_share_eachkey**] `*Required*`

`true` Send notification each key scanned (Required `telegram_share=true`)

`false` It only sends a notification when the private key is found. (Required `telegram_share=true`)

---

### [**untrusted_computer**] `*Required*`

Leave true if you are working on an untrusted computer

`true` When private key is found, it only sends it via Telegram. Make sure your Telegram settings are correct. Otherwise, when the key is found, you will not be able to see it anywhere.

`false` When private key is found, The private key will be saved in a new text file. If Telegram share is active, notification will be sent.

---

2000000-2050000 (First ~%0.98) ranges and 3FAF000-3FFFFFF (Last ~%0.98) manually defeated in this pool. If you rescan a defeated range, it will now be marked as scanned normal.

## Example

Random key from database: **326FB80**

The program tells Bitcrack to scan the following range: **326FB800000000000** / **326FB810000000000**

When the range is scanned, a new private key is requested and the process proceeds in this way.

# How to use?

Download released version or build it yourself.

Download [Bitcrack](https://github.com/brichard19/BitCrack) or build it yourself (recommended) or download it from this repo (bitcrack.zip) (recommended)

Download .NET 6.0 runtimes from [Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

Edit the settings.txt file according to you.

Run the application.

# If found?

If the private key is found, it will appear on the console screen. Also, a new text file will be created in the folder where the application is run. (in the name of the destination wallet address.)

# Information

1. This is not a shared pool. However, it can be done in the future.
2. If the private key is found, only you can see it. No one else can see!
3. If the private key is found, it is not shared.
4. Once a private key is scanned, it is not scanned again.
5. You can see the percentage on the application.
6. If you exit the application before the scan is not complete, the scanned HEX value will not be marked as scanned.
7. Your luck; One in 33 million.

# Try it on Vast.ai (Bitcoin accepted)

Register [Vast.ai](https://cloud.vast.ai/?ref=69296) to rent GPU(s).

(1) Select custom docker image `nvidia/cuda:12.0.0-devel-ubuntu20.04` in "Template Slot" and run any instance. (3090 or 4090 is fine)

(2) Connect instance via SSH client (Like PuTTY). Follow commands:

`sudo apt install nano` Install optional app (For edit settings.txt file)

`wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh` Installing .NET core runtime for linux

`sudo chmod +x ./dotnet-install.sh`

`./dotnet-install.sh --version latest`

`export DOTNET_ROOT=$HOME/.dotnet`

`export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools`

`export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`

`git clone https://github.com/brichard19/BitCrack` Cloning BitCrack app

`cd BitCrack`

`make BUILD_CUDA=1 COMPUTE_CAP=86` Building BitCrack.

`git clone https://github.com/ilkerccom/bitcrackrandomiser` Cloning BitcrackRandomiser

`cd bitcrackrandomiser/BitcrackRandomiser`

Edit settings.txt file. You can edit settings.txt file with `nano settings.txt` or connect with WinSCP and download-edit *settings.txt* file. Example below:

> bitcrack_path=/root/BitCrack/bin/./cuBitCrack
> 
> bitcrack_arguments=-b 896 -t 256 -p 256
> 
> wallet_address=1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw
> 
> scan_type=default
> 
> custom_range=none
> 
> telegram_share=false
> 
> telegram_acesstoken=0
> 
> telegram_chatid=0
> 
> telegram_share_eachkey=false
>
> untrusted_computer=false

`dotnet run` Run the bitcrackrandomiser

You can press "CTRL+B" and then "D" to leave terminal without closing app.

# More info

If 15,000 people scan 1.7 billion per second (like 3090), it would take 15 days to scan all keys.

**I will wrap up more information here soon.**

# Donate 

1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw


