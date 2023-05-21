
# Bitcrack-Randomiser

**Bitcrack-Randomiser** works integrated with the application called Bitcrack. 

![alt text](https://i.ibb.co/DGq2Bbz/ap.png)

## How it works?

It only works with BTC Puzzle 66. If you want to **scan all private keys in  puzzle 66**; you need to do 36 quintillion scans in total. In case you do a random scan; previously generated private keys will be regenerated (random problem). This extends the scan time by x10.

## Puzzle 66

Puzzle 66 HEX ranges as follows. It starts with 2 or 3. Any private key in this range is **17 characters long.**

`20000000000000000 to
3ffffffffffffffff`

**We take the first 7 characters** and delete the rest for now. The result will be as follows.

`2000000 to
3ffffff`

We now have about 33 million possible private keys to search. All possible private keys are **stored in the database**. A random value will come up each time a scan job is called and **will be marked as scanned** when the scan is complete. 

I can scan each key in about 10 minutes on NVIDIA 3090. This actually means about 1,1 trillion private keys. When the private key is scanned, it is marked as scanned. So it won't show up anymore.



## settings.txt

Add the folder where the Bitcrack application is located in the first line. **Note: Only for NVIDIA CUDA devices. Do not use clBitCrack.exe**

`C:\Users\{YOURUSERNAME}\App\bitcrack\cuBitCrack.exe`

You can write the arguments for Bitcrack. Do not change the -o and -keyspace arguments.

`-b 896 -t 256 -p 256 -o {2}.txt --keyspace {0}0000000000:{1}0000000000 {2}`

Enter the BTC address here. Remember for now **this is not a pool**! But if in the future please enter a BTC recipient address.

`1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw`

## Example

Random key from database: **326FB80**

The program tells Bitcrack to scan the following range: **326FB800000000000** / **326FB810000000000**

When the range is scanned, a new private key is requested and the process proceeds in this way.

# How to use?

Download released version or build it yourself.

Download .NET 6.0 runtimes from [Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

Edit the settings.txt file according to you.

Run the application.

# If found?

If the private key is found, it will appear on the console screen. Also, a new text file will be created in the folder where the application is run. (in the name of the destination wallet address.)

# Information

1. This is not a pool. However, it can be done in the future.
2. If the private key is found, only you can see it. No one else can see!
3. If the private key is found, it is not shared.
4. Once a private key is scanned, it is not scanned again.
5. You can see the percentage on the application.
6. If you exit the application before the scan is not complete, the scanned HEX value will not be marked as scanned.
7. Your luck; One in 33 million.

# More info

If 15,000 people scan 1.7 billion per second (like 3090), it would take 15 days to scan all keys.

**I will wrap up more information here soon.**

# Donate 

1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw


