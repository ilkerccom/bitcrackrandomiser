# Bitcrack-Randomiser

Bitcrackrandomiser is a solo pool for Bitcoin puzzle **66, 67 and 68**. It works with Bitcrack.

Supports <ins>Windows</ins>, <ins>Linux</ins> and <ins>MacOS</ins>.

Supports <ins>NVIDIA</ins> and <ins>AMD</ins> devices. (**AMD Bitcrack v0.30 only**)

![alt text](https://i.ibb.co/vYHYsMq/bitcrackrandomiser.png)

## Related Links

Website | Link
--- | ---
Pool website | [btcpuzzle.info](https://btcpuzzle.info/) 
Support | [t.me/bitcrackrandomiser](https://t.me/bitcrackrandomiser)
Github repo | [github.com/bitcrackrandomiser](https://github.com/ilkerccom/bitcrackrandomiser)
API Documentation | [API-DOCUMENTATION.MD](https://github.com/ilkerccom/bitcrackrandomiser/blob/main/API-DOCUMENTATION.MD)


## How it works?

It only works with BTC Puzzle 66, 67 and 68 (You can change the puzzle number from the **[<ins>target_puzzle</ins>]** variable in the <ins>[settings.txt](./BitcrackRandomiser/settings.txt)</ins> file.).

## Proof of Work

The pool always returns you a random **3 addresses** on every scan job. The private key of these addresses is scanned simultaneously. When the private key of the any address is found, it saves it as "**ProofKey**". This is to make sure your program is working correctly. I also want to make sure you have a really healthy scan.

Example; pool returns `3E2ECB0` HEX value to scan. The pool randomly generates <ins>extra 3 private keys</ins> within the returned HEX range. `3E2ECB00000000000` and `3E2ECB0FFFFFFFFFF`. 

Marking is done with `SHA256(PROOFKEY1+PROOFKEY2+PROOFKEY3)`

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

For Puzzle 66: 2000000-2050000 (First ~%0.98) ranges and 3FAF000-3FFFFFF (Last ~%0.98) manually defeated in this pool. If you rescan a defeated range, it will now be marked as scanned normal.

## Example

Random key from database: **326FB80**

The program tells Bitcrack to scan the following range: **326FB800000000000** / **326FB810000000000**

When the range is scanned, a new private key is requested and the process proceeds in this way.

# How to use?

You can read detailed [How To Use Guide](https://github.com/ilkerccom/bitcrackrandomiser/blob/main/HOW-TO-USE.md)

### Simple Using

1 - Create an account on [BTCPuzzle.info](https://btcpuzzle.info/dashboard) website and obtain your user token.

2 - Download latest released [Bitcrackrandomiser](https://github.com/ilkerccom/bitcrackrandomiser/releases) or build it yourself.

3 - Download .NET 6.0 runtimes from [Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

4 - Edit the <ins>[settings.txt](./BitcrackRandomiser/settings.txt)</ins> file according to you.

5 - Run the application.

### Docker Image Using

You can use docker image for a faster experience. You can also create your own docker image. "[Dockerfile](./Dockerfile)" is available in the repo. Visit the [Bitcrackrandomiser Docker Images](https://hub.docker.com/r/ilkercndk/bitcrackrandomiser/tags)

<ins>What needs to be done above is ready in the Docker image. All you have to do is run the application.</ins>



### ilkercndk/bitcrackrandomiser:latest

Everything is ready! Edit the [settings.txt](./BitcrackRandomiser/settings.txt) file and run the app!

```bash
$ docker run -it ilkercndk/bitcrackrandomiser:latest
... edit settings file ...
$ dotnet BitcrackRandomiser.dll
```

### ilkercndk/bitcrackrandomiser:autorun

Everything is ready! When you run the image, <ins>bitcrackrandomiser</ins> starts automatically. You can see the Docker create/run settings in the example below. Does not need interactive console.

```bash
$ docker run -e BC_WALLET=xxxx -e BC_USERTOKEN=xxxx ilkercndk/bitcrackrandomiser:autorun
```

# Settings

You can update the application settings via the "[settings.txt](./BitcrackRandomiser/settings.txt)" file or in app. You can pass arguments to the application as in the example below.

```
dotnet BitcrackRandomiser.dll target_puzzle=66 user_token=xxxx wallet_address=1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw ...any other settings
```

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

Possible Value|Description
-|-
`cuBitcrack`|For NVidia Cuda devices
`clBitcrack`|For AMD devices
`C:\{BITCRACK_PATH}\cuBitCrack.exe`|Example custom path on Windows
`{BITCRACK_PATH}/./cuBitCrack`|Example custom path on Linux

**NOTE: You can use OpenCL "clBitCrack.exe" for <ins>AMD devices on Bitcrack v0.30 only</ins>**

---

### [**app_arguments**] 

You can write the arguments for Bitcrack. For default settings leave blank.

`-b 896 -t 256 -p 256` or `-t 128` or you can leave blank.

Note: Do not use `-o --keyspace` parameters.

---

### [**user_token**]

<ins>You can create user token value by logging into your account</ins> at btcpuzzle.info. If you do not have an account, you can create a new account using your wallet address.

You can revoke the user token value at any time. However, when you do this, you must also enter the new value from the workers settings file.

Example user token value;

`VDGcruTrDZ62EuJsE9IQUCiRIKRhZpXw6RPtcnk1jBxbROn1nxZixBMql8L2zxKwD9QXb1UZoWgrDf8IwciRDUHxHzwkNrDzNBpio2UdAx4rLYsjMnI887eqWGauszBl`


---

### [**wallet_address**]

Enter the BTC wallet address here. 

`1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw` or `1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw.worker1`

You can specify a **worker name** like `{wallet}.{worker}` Only alphanumeric is accepted. Max 16 characters. Do not use special characters. If you do not enter a worker name, it will be created automatically.

If you enter an invalid BTC address, it will show as "Unknown" in the system.

**Note: You can only enter your wallet address registered to your membership account. If you enter any other address, you will get an error.**

---

### [**scan_type**]

Select a scan type. 

Possible Value|Description
-|-
`default`|Scan anything that isn't scanned.
`includeDefeatedRanges`|Include defeated ranges. This does not scan ranges that have already been scanned!
`excludeIterated2`|Exclude iterated ranges (2 iterated and more). Not good choice. Example: 1A<ins>FF</ins>1B3 
`excludeIterated3`|Exclude iterated ranges (3 iterated and more). May be good choice. Example: 1A<ins>FFF</ins>B3
`excludeIterated4`|Exclude iterated ranges (4 iterated and more). Good choice. Example: 1A<ins>FFFF</ins>3
`excludeContains3`|Exclude iterated ranges (3 and more). Not good choice. Example: 1<ins>F</ins>A1<ins>FF</ins>3
`excludeContains4`|Exclude iterated ranges (4 and more). May be good choice. Example: 1<ins>F</ins>A<ins>F</ins>C<ins>FF</ins>
`excludeAlphanumericLoop`|Exclude if HEX contains only numbers or only letters. Example: <ins>2572441</ins> or <ins>BCAFFEB</ins>
`excludeEven` <br/>or<br/> `excludeOdd`|Exclude the even or odd numbered HEX range.<br/><br/>You can only choose one. When you select any of them, the number of keys to be scanned decreases by 1/2.
`excludeStartsWith{XX}`|Exclude ranges that starts with. [Min 1, max 2 chars]. <br/><br/>Example [1]: `excludeStartsWith2` The range starting with 2 will not return.<br/>Example [2]: `excludeStartsWith2A` The range starting with 2A will not return.<br/><br/>If you enter a value that you entered in the "**custom_range**" field, you will get a "reached of keyspace" error and the application will be stopped.

You can make multiple settings using commas.

```
...
scan_type=excludeIterated3,excludeStartsWith2A,excludeStartsWith2B,excludeStartsWith3F
...
```

---

### [**custom_range**]

Scan custom range

`none` Scan all of ranges.

`2D` or `3BA` or `3FF1`  Enter the first [2-5] characters of the range you want to scan. Only one value.

You can use specify ranges like `3400000:38FFFFF`. Incoming keys will be selected from this range. You must write the range in full length. Make sure you enter the correct range.

You can use percentiles like `%5:%20`. In the example, only HEX values between 5% and 20% will be scanned.

Note: You can specify only one setting for the custom range field.

---

### [**api_share**]

`none` or `https://{your_api_url}`

Receive the all actions as a POST request to your own server. All values are requested as "header". Below you can see what data is coming.

```C
status // [workerStarted, workerExited, rangeScanned, reachedOfKeySpace, keyFound]
hex // Scanned HEX value
privatekey // Private key if that found
targetpuzzle // Which puzzle is being scanned [66,67,68 or 38]
workeraddress // Worker wallet address [1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw]
workername // Worker name [worker1039]
scantype // Current scan type [default, includeDefeatedRanges]
```

I wrote a sample PHP script to get the data. It sends info to Telegram.

```php
<?php
$headers = getallheaders();
$status = $headers['Status'];
$hex = $headers['Hex'];
$workeraddress = $headers['Workeraddress'];
$workername = $headers['Workername'];
$privatekey = $headers['Privatekey'];
$scantype = $headers['Scantype'];
$targetpuzzle = $headers['Targetpuzzle'];

if($status == "workerStarted"){
	shareTelegram($workeraddress.$workername." started job!");
}
else if($status == "workerExited"){
	shareTelegram($workeraddress.$workername." goes offline!");
}
else if($status == "rangeScanned"){
	shareTelegram($hex." scanned by ".$workeraddress.$workername);
}
else if($status == "reachedOfKeySpace"){
	shareTelegram($workeraddress.$workername." reached of keyspace!");
}
else if($status == "keyFound"){
	shareTelegram("Congratulations! ".$workeraddress.$workername." found the key! Key is: ".$privatekey);
}
function shareTelegram($message){
	$apiToken = "{telegram_api_token}";
	$chatId= "{telegram_chat_id}";
	$data = [ 
	 "chat_id" => $chatId, 
	 "text" => $message
	]; 
	$response = file_get_contents("http://api.telegram.org/bot$apiToken/sendMessage?" . http_build_query($data) ); 
}
?>
```

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
```

<ins>[TargetAddress]</ins> The private key you want to find

<ins>[HexStart]</ins> HEX range to start scanning

<ins>[HexEnd]</ins> HEX range to stop scanning


---

### [**force_continue**] 

`true` If the private key is found, the scan will continue until it is finished. The related range is marked as "scanned". The key found is publicly visible on the pool site. (With 1 missing character) 

You can see the private key in the file created in the folder and if Telegram is active, notification will come.

`false` If the private key is found, the scanning process is terminated. No data is sent to the pool.


---

### [**private_pool**] 

`none` or `{private_pool_id}`

You can create your own pool for Puzzle 66, 67 and 68. Only the user who created this pool can use it and no other user can join.

Private pools use a database other than the main pool. That's why it's completely empty. Ranges scanned in the private pool just stay there. It is not transferred to another pool.

Private pools can be created for $10 per month. You can reach me via Telegram for create your own puzzle pool.


# If found?

`untrusted_computer=false` If the private key is found, it will appear on the console screen. Also, a new text file will be created in the folder where the application is run. (in the name of the target wallet address.)

`untrusted_computer=true` If the private key is found, it will send your Telegram channel/group only.

# General Information

1. This is <ins>not a shared pool</ins>.
2. If the private key is found, <ins>only you can see it</ins>. No one else can see!
3. If the private key is found, <ins>it is not shared</ins>.
4. Once a private key is scanned, it is not scanned again.
5. You can see the percentage on the application.
6. If you exit the application before the scan is not complete, the scanned HEX value will not be marked as scanned.
7. Your luck; One in 33 million.

# Donate
`1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw`


