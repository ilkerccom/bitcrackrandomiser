
# Bitcrack-Randomiser

Bitcrackrandomiser is a solo pool for Bitcoin puzzle **66, 67 and 68**. It works with Bitcrack.

Supports <ins>Windows</ins>, <ins>Linux</ins> and <ins>MacOS</ins>.

Supports <ins>NVIDIA</ins> and <ins>AMD</ins> devices. (**AMD Bitcrack v0.30 only**)

![alt text](https://i.ibb.co/vYHYsMq/bitcrackrandomiser.png)

## Pool & Support

Go to pool > https://btcpuzzle.info/

For support > https://t.me/bitcrackrandomiser

## How it works?

It only works with BTC Puzzle 66,67 and 68 (You can change the puzzle number from the **[<ins>target_puzzle</ins>]** variable in the <ins>settings.txt</ins> file.).

## Proof of Work

The pool always returns you a random secondary address on every scan job. The private key of secondary  address is scanned simultaneously. When the private key of the secondary address is found, it saves it as "**ProofKey**". This is to make sure your program is working correctly. I also want to make sure you have a really healthy scan.

Example; pool returns `3E2ECB0` HEX value to scan. The pool randomly generates another private key within the returned HEX range. `3E2ECB00000000000` and `3E2ECB0FFFFFFFFFF`. 

When retrieving a random HEX value from the pool, the wallet address of the secondary private key is additionally returned. Marking is done with the secondary private key found.

"**ProofKey**" is used only for data marking.

## Example Puzzle 66 Scenario

If you want to **scan all private keys in  puzzle 66**; you need to do 36 quintillion scans in total. In case you do a random scan; previously generated private keys will be regenerated (random problem). This extends the scan time by x10. Puzzle 66 HEX ranges as follows. It starts with 2 or 3. Any private key in this range is **17 characters long.**

`20000000000000000 to
3ffffffffffffffff`

**We take the first 7 characters** and delete the rest for now. The result will be as follows.

`2000000 to
3ffffff`

We now have about 33 million possible private keys to search. All possible private keys are **stored in the database**. A random value will come up each time a scan job is called and **will be marked as scanned** when the scan is complete. 

I can scan each key in about 10 minutes on NVIDIA 3090. This actually means about 1,1 trillion private keys. When the private key is scanned, it is marked as scanned. So it won't show up anymore.

## Settings

You can update the application settings via the "settings.txt" file or in app.

---

### [**target_puzzle**]

Select the puzzle you want to scan. Default: 66

`66` or `67` or `68`

You can use `38` for test pool. There are 32 possible ranges in the test pool. You can find the test pool data on the website.Test pool data is reset every 30 minutes.

---

### [**app_type**]

Currently only `bitcrack` is available.

---

### [**app_path**]

Add the folder where the Bitcrack application is located in the first line.

On Windows: `C:\{BITCRACK_PATH}\cuBitCrack.exe`

On Linux: `/root/{BITCRACK_PATH}/bin/./cuBitCrack`

**NOTE: You can use OpenCL "clBitCrack.exe" for <ins>AMD devices on Bitcrack v0.30 only</ins>**

---

### [**app_arguments**] 

You can write the arguments for Bitcrack. For default settings leave blank.

`-b 896 -t 256 -p 256` or `-t 128` or you can leave blank.

Note: Do not use `-o --keyspace` parameters.

---

### [**wallet_address**]

Enter the BTC wallet address here. 

`1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw` or `1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw.worker1`

You can specify a **worker name** like `{wallet}.{worker}` Only alphanumeric is accepted. Max 16 characters. Do not use special characters. If you do not enter a worker name, it will be created automatically.

If you enter an invalid BTC address, it will show as "Unknown" in the system.

---

### [**scantype**]

Select a scan type.

`default` - Scan and exclude defeated ranges.

`includeDefeatedRanges` - Scan and include defeated ranges.

---

### [**custom_range**]

Scan custom range

`none` Scan all of ranges.

`2D` or `3BA` or `3FF1`  Enter the first [2-5] characters of the range you want to scan.

You can now specify ranges like `3400000:38FFFFF`. Incoming keys will be selected from this range. You must write the range in full length. Make sure you enter the correct range.

---

### [**telegram_share**]

Share progress to Telegram

`true` Share progress to Telegram. 

`false` If false, it does not send notification. 

It sends a notification to Telegram when the private key is found. If you set "**telegram_share_eachkey**" to "true", it will send notification every time the scan is finished.

If your Telegram settings are correct, you will receive a notification that the worker has started working.

Suggestion: If you are on an untrusted computer, make the settings via the console and proceed without saving.

---

### [**telegram_acesstoken**]

Enter Telegram BOT access token

Example: 6331494066:ABEfv4cF3dbc3mA8qGLDlEp2uxzgYESIa_w

---

### [**telegram_chatid**]

Enter Telegram chat id

Example: -9334716240

---

### [**telegram_share_eachkey**]

`true` Send notification each key scanned.

`false` It only sends a notification when the private key is found.

---

### [**untrusted_computer**]

Leave true if you are working on an untrusted computer

`true` When private key is found, <ins>**it only sends it via Telegram**</ins>. Make sure your Telegram settings are correct. <ins>Otherwise, when the key is found, you will not be able to see it anywhere.</ins>

`false` When private key is found, The private key will be <ins>saved in a new text file</ins> and it <ins>appears on console screen</ins>. If Telegram share is active, notification will be sent.



---

### [**test_mode**] 

Start app in test mode if `true`. You can test with custom parameters by creating a "**customtest.txt**" file in the app root folder.

```C
1Cnrx6rxiGvVNw1UroYM5hRjVvqPnWC7fR // [TargetAddress]
2012E83 // [HexStart]
2012E84 // [HexEnd]
1Hz8wCQp9j71j8NGuzFE5KN9SV7PeRguai // [ProofValue]
```

<ins>[TargetAddress]</ins> The private key you want to find

<ins>[HexStart]</ins> HEX range to start scanning

<ins>[HexEnd]</ins> HEX range to stop scanning

<ins>[ProofValue]</ins> Proof of work value. Example: If [HexStart] = `2012E84`, create a BTC address in range `2012E840000000000` and `2012E84FFFFFFFFFF`.


---

For Puzzle 66: 2000000-2050000 (First ~%0.98) ranges and 3FAF000-3FFFFFF (Last ~%0.98) manually defeated in this pool. If you rescan a defeated range, it will now be marked as scanned normal.

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

`untrusted_computer=false` If the private key is found, it will appear on the console screen. Also, a new text file will be created in the folder where the application is run. (in the name of the target wallet address.)

---

`untrusted_computer=true` If the private key is found, it will send your Telegram channel/group only.


# Information

1. This is <ins>not a shared pool</ins>.
2. If the private key is found, <ins>only you can see it</ins>. No one else can see!
3. If the private key is found, <ins>it is not shared</ins>.
4. Once a private key is scanned, it is not scanned again.
5. You can see the percentage on the application.
6. If you exit the application before the scan is not complete, the scanned HEX value will not be marked as scanned.
7. Your luck; One in 33 million.

# Try it on Vast.ai (Bitcoin accepted)

Register [Vast.ai](https://cloud.vast.ai/?ref=69296) to rent GPU(s).

(1) Select custom docker image `nvidia/cuda:12.0.0-devel-ubuntu20.04` in "Template Slot" and run any instance. (3090 or 4090 is fine)

(2) Connect instance via SSH client (Like PuTTY). Follow commands:

```bash
$ sudo apt install nano
$ wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
$ sudo chmod +x ./dotnet-install.sh
$ ./dotnet-install.sh --version latest
$ export DOTNET_ROOT=$HOME/.dotnet
$ export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools
$ export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
$ git clone https://github.com/brichard19/BitCrack
$ cd BitCrack
$ make BUILD_CUDA=1 COMPUTE_CAP=86
$ git clone https://github.com/ilkerccom/bitcrackrandomiser
$ cd bitcrackrandomiser/BitcrackRandomiser
```

Edit settings.txt file. You can edit settings.txt file with `nano settings.txt` or connect with WinSCP and download-edit *settings.txt* file. Example below:

```
target_puzzle=66
app_type=bitcrack
bitcrack_path=/root/BitCrack/bin/./cuBitCrack
bitcrack_arguments=-b 896 -t 256 -p 256
wallet_address=1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw
... other settings
```

Finally, run the app.

```
$ dotnet run
```

You can press "<ins>CTRL+B</ins>" and then "<ins>D</ins>" to leave terminal without closing app.

# Donate 

1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw


