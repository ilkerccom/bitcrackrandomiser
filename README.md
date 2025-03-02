# Bitcrack-Randomiser

Bitcrackrandomiser is a solo pool project for Bitcoin puzzle **68, 69 and 71**. (Technically supports up to Puzzle 160). It works with Bitcrack and VanitySearch.

Official client for "btcpuzzle.info pool".

Supports <ins>Windows</ins> and <ins>Linux</ins>. Supports <ins>NVIDIA</ins> / <ins>AMD</ins> devices and <ins>CPU</ins>. (**AMD Bitcrack v0.30 only, CPU VanitySearch only**)

![alt text](https://i.ibb.co/sC3KDxB/app.png)

## Related Links

Website | Link | Name
--- | --- | ---
Pool website | [btcpuzzle.info](https://btcpuzzle.info/) | ![btcpuzzle.info logo](https://i.ibb.co/XLWWn8G/btcpuzzle-info-120.png)
Support | [t.me/bitcrackrandomiser](https://t.me/bitcrackrandomiser) | ![btcpuzzle.info logo](https://i.ibb.co/XLWWn8G/btcpuzzle-info-120.png)
Github repo | [github.com/bitcrackrandomiser](https://github.com/ilkerccom/bitcrackrandomiser) | -
Docker images | [hub.docker.com/u/ilkercndk](https://hub.docker.com/r/ilkercndk/bitcrackrandomiser) | -
API Documentation | [API-DOCUMENTATION.MD](https://github.com/ilkerccom/bitcrackrandomiser/blob/main/API-DOCUMENTATION.MD) | -

### Build it yourself

You can build the client and all the applications used yourself. %100 open-source client & apps.

![build it yourself](https://i.ibb.co/7Gv3ZRk/b350.png)

- Bitcrack ([Go repo](https://github.com/brichard19/BitCrack))
- VanitySearch ([Go repo](https://github.com/ilkerccom/VanitySearch))
- VanitySearch (Optimised) ([Go repo](https://github.com/ilkerccom/VanitySearch-V2))
- Bitcrackrandomiser (This repo)

Endless thanks to everyone involved in the development of Bitcrack and VanitySearch applications.


## How it works?

It only works with BTC Puzzle 68, 69 and 71 (You can change the puzzle number from the **[<ins>target_puzzle</ins>]** variable in the <ins>[settings.txt](./BitcrackRandomiser/settings.example.txt)</ins> file.).

## Proof of Work

When requesting a range from the pool, **three wallet addresses** are also returned. The private key of these addresses is scanned simultaneously. To ensure that a range is scanned, the private key of three wallet addresses must be found. The private keys of the found addresses are hashed with SHA256. In this way, **"Proof Key"** is created. This is to make sure your program is working correctly. I also want to make sure you have a really healthy scan.

Example; pool returns `3E2ECB0` HEX range to scan. The pool randomly generates <ins>extra 6 private keys</ins> within the returned HEX range. `3E2ECB00000000000` and `3E2ECB0FFFFFFFFFF`. 

Marking is done with `SHA256(PROOFKEY1+PROOFKEY2+PROOFKEY3+PROOFKEY4+PROOFKEY5+PROOFKEY6)`

<ins>**Note:** The number of proof keys can be increased/decreased dynamically by the API. By standard the total number is 6.</ins>

## Example Puzzle 68 Scenario

If you want to **scan all private keys in  puzzle 68**; you need to do ~143 quintillion (143,172,492,000,000,000) scans in total. In case you do a random scan; previously generated private keys will be regenerated (random problem). This extends the scan time by min. x10. Puzzle 68 HEX ranges as follows. Any private key in this range is **17 characters long.**

`80000000000000000 to
fffffffffffffffff`

**We take the first 7 characters** and delete the rest for now. *Some puzzles can be 8 characters or more. The result will be as follows. 

`8000000 to
fffffff`

We now have about ~33 million possible ranges to search. (If you convert this HEX range to decimal format, it actually becomes 134 million ranges. However, since each range contains 4 times the private key, we set it to 33 million.) All possible ranges are **stored in the database**. A random value will come up each time a scan job is called and **will be marked as scanned** when the scan is complete. 

***Note: Each range contains 4,4 trillion private keys in Puzzle 68.*** You can find information such as how many keys each puzzle contains on the website.

This actually means about 4,4 trillion private keys. When the range is scanned, it is marked as scanned. So it won't show up anymore.

## Example

Random range from database: **926FB80**

The program tells Bitcrack/VanitySearch to scan the following range: 

**926FB800000000000** / **926FB83FFFFFFFFFF** (Contains 4,4 trillion private keys) (*Note that we added +4 to the starting HEX value. 926FB80:926FB84)

When the range is scanned, a new range is requested and the process proceeds in this way.

# How to use?

You can read detailed [How To Use Guide](https://github.com/ilkerccom/bitcrackrandomiser/blob/main/HOW-TO-USE.md)

## Simple Using

1 - Create an account on [BTCPuzzle.info](https://btcpuzzle.info/user-center) website and obtain your user token.

2 - Download latest released [Bitcrackrandomiser](https://github.com/ilkerccom/bitcrackrandomiser/releases) or build it yourself.

3 - Download .NET 8.0 runtimes from [Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (.NET Runtime 8.x.x)

4 - Edit the <ins>[settings.example.txt](./BitcrackRandomiser/settings.example.txt)</ins> file according to you.

5 - Run the application.

## Using CloudSearch

If you're looking for the easiest way to join the pool, you're in the right place. Rent the graphics card you want at hourly rates and chase the reward. Go to [CloudSearch by btcpuzzle.info](https://btcpuzzle.info/cloud-search).

1 - Create an account on [BTCPuzzle.info](https://btcpuzzle.info/user-center) website and activate your CloudSearch account.

2 - Top up balance (USD) using Polygon, Base, ETH and many other networks.

3 - Create a template and rent the instance you want. That is all!

## Docker Images

You can use docker image for a faster experience. You can also create your own docker image. "[Dockerfile](./Docker/Dockerfile)" is available in the repo. Visit the [Bitcrackrandomiser Docker Images](https://hub.docker.com/r/ilkercndk/bitcrackrandomiser/tags)

<ins>What needs to be done above is ready in the Docker image. All you have to do is run the application.</ins>

- <ins>***ilkercndk/bitcrackrandomiser:latest***</ins> -> Default vanitysearch (Supports up to RTX 4000 series)
  - The default vanitysearch is used. You can scan with multiple GPUs on a range.
  - It scans slower than the optimized VanitySearch.
  - Supports maximum RTX 4000 series GPUs.
- <ins>***ilkercndk/bitcrackrandomiser:cuda-122***</ins> -> Optimized vanitysearch (Supports up to RTX 4000 series)
  - The default vanitysearch is used. A maximum of 1 GPU can be used for each range.
  - It scans faster than the default VanitySearch.
  - Supports maximum RTX 4000 series GPUs.
- <ins>***ilkercndk/bitcrackrandomiser:cuda-128***</ins> -> Optimized vanitysearch (Supports RTX 5000 series)
  - The default vanitysearch is used. A maximum of 1 GPU can be used for each range.
  - It scans faster than the default VanitySearch.
  - Supports maximum RTX 5000 series GPUs.

### # ilkercndk/bitcrackrandomiser:latest

Everything is ready! Edit the [settings.txt](./BitcrackRandomiser/settings.example.txt) file and run the app!

```bash
$ docker run -it ilkercndk/bitcrackrandomiser:latest
... edit settings file ...
$ dotnet BitcrackRandomiser.dll
```

Don't forget to add the `--gpus all` flag when running on your local computer.

```bash
$ docker run --gpus all -it ilkercndk/bitcrackrandomiser:latest
```
Using env. variables

```bash
$ docker run --gpus all -it ilkercndk/bitcrackrandomiser:latest -e BC_USERTOKEN=xxx -e BC_WORKER=workername ...
```
You can add auto start script

```bash
bash /app/bitcrackrandomiser/bitcrackrandomiser.sh
```

### Docker options with default settings

```bash
BC_PUZZLE=68
BC_USERTOKEN="0"
BC_WORKERNAME=""
BC_APP_TYPE="vanitysearch"
BC_APP="/app/VanitySearch/./vanitysearch"
BC_APP_ARGS=""
BC_GPUCOUNT="1"
BC_GPUINDEX="0"
BC_GPUSEPERATEDRANGE="true"
BC_CUSTOM_RANGE="none"
BC_API_SHARE="none"
BC_TELEGRAM_SHARE="false"
BC_TELEGRAM_ACCESS_TOKEN="0"
BC_TELEGRAM_CHAT_ID="0"
BC_TELEGRAM_SHARE_EACHKEY="false"
BC_UNTRUSTED_COMPUTER="false"
BC_FORCE_CONTINUE="false"
BC_CLOUDSEARCHMODE="true"
```

# Settings

You can update the application settings via the "[settings.txt](./BitcrackRandomiser/settings.example.txt)" file or in app. You can create your settings file on btcpuzzle.info dashboard.

Also, You can pass arguments to the application as in the example below.

```
dotnet BitcrackRandomiser.dll target_puzzle=68 user_token=xxxx ...any other settings
```

---

### [**target_puzzle**]

Select the puzzle you want to scan.

`68` or `69` or `71` or `current any puzzle`

You can use `38` for test pool. There are 32 possible ranges in the test pool. You can find the test pool data on the website. Test pool data is reset every 30 minutes.

---

### [**app_type**]

Currently only `bitcrack` and `vanitysearch` is available.

Possible Value|Description
-|-
`bitcrack`|Scan with Bitcrack
`vanitysearch`|Scan with VanitySearch
`cpu`|Scan with CPU only

---

### [**app_path**]

Possible Value|Description
-|-
`cuBitcrack`|For NVidia Cuda devices
`clBitcrack`|For AMD devices
`vanitysearch`|For VanitySearch and CPU support
`C:\{BITCRACK_PATH}\cuBitCrack.exe`|Example custom path on Windows
`{BITCRACK_PATH}/./cuBitCrack`|Example custom path on Linux

**NOTE: You can use OpenCL "clBitCrack.exe" for <ins>AMD devices on Bitcrack v0.30 only</ins>**

---

### [**app_arguments**] 

You can write the arguments for Bitcrack or VanitySearch. For default settings leave blank.

`-b 896 -t 256 -p 256` or `-t 128` or you can leave blank.

Note: Do not use `-o --keyspace` parameters.

---

### [**user_token**] *Required

<ins>You can create user token value by logging into your account</ins> at btcpuzzle.info. If you do not have an account, you can create a new account using your wallet address.

You can revoke the user token value at any time. However, when you do this, you must also enter the new value from the workers settings file.

Example user token value;

`VDGcruTrDZ62EuJsE9IQUCiRIKRhZpXw6RPtcnk1jBxbROn1nxZixBMql8L2zxKwD9QXb1UZoWgrDf8IwciRDUHxHzwkNrDzNBpio2UdAx4rLYsjMnI887eqWGauszBl`


---

### [**worker_name**]

Enter the worker name.

`worker4124` or `anyworkername`

Only alphanumeric is accepted. Max 16 characters. Do not use special characters. If you do not enter a worker name, it will be created automatically.


---

### [**gpu_count**]

Specify the number of GPUs to use. Default: 1. 

Each GPU performs a separate range scan.

If you are using more than one GPU, GPU indexes will be automatically added to the end of your worker address. Example: `{worker}_0` or `{worker}_1`

---

### [**gpu_index**]

Index number of the graphics card to be used. Use only if you are going to scan with a single graphics card.

If you have a single video card, use the value "0". If you are scanning with more than one graphics card, this field is disabled.

---

### [**gpu_seperated_range**]

Only valid for VanitySearch. By default, VanitySearch uses 1 range to scan all graphics cards. If you set this setting to "true", each GPU scans different ranges. 

In some systems with multi GPU, it works slightly better than the default setting.

You must have at least 2 GPUs to use this setting. You also need to update the "gpu_count" setting according to the number of graphics cards you have.


---

### [**scan_rewards**]

`true` or `false`

It scans "pool rewards" addresses in addition to the target wallet address. If it finds the reward key it just creates a new text file and sends notification via telegram or api_share.

Even if the reward is found, the application continues to operate normally. The reward found can only be claimed by the person who scans the range where the private key is located.


Click for detailed **"[reward creation and claiming](https://btcpuzzle.info/reward.pdf)"**  diagram in PDF.

---

### [**custom_range**]

Scan custom range

`none` Scan all of ranges.

`2D` or `3BA` or `3FF1`  Enter the first [1-7] characters of the range you want to scan. Only one value.

You can use specify ranges like `3400000:38FFFFF`. Incoming keys will be selected from this range. You must write the range in full length. Make sure you enter the correct range.

Note: You can specify only one setting for the custom range field.

---

### [**api_share**]

`none` or `https://{your_api_url}`

Receive the all actions as a POST request to your own server. All values are requested as "header". Below you can see what data is coming.

IMPORTANT: If the key is found and an unsuccessful result is received from the API (body), it makes a unlimited attempts.

```C
status // [workerStarted, workerExited, rangeScanned, reachedOfKeySpace, keyFound]
hex // Scanned HEX value
privatekey // Private key if that found
targetpuzzle // Which puzzle is being scanned
workername // Worker name [worker1039]
```

I wrote a sample PHP script to get the data. It sends info to Telegram.

```php
<?php
$headers = getallheaders();
$status = $headers['Status'];
$hex = $headers['Hex'];
$workername = $headers['Workername'];
$privatekey = $headers['Privatekey'];
$targetpuzzle = $headers['Targetpuzzle'];

if($status == "workerStarted"){
	shareTelegram($workername." started job!");
}
else if($status == "workerExited"){
	shareTelegram($workername." goes offline!");
}
else if($status == "rangeScanned"){
	shareTelegram($hex." scanned by ".$workername);
}
else if($status == "reachedOfKeySpace"){
	shareTelegram($workername." reached of keyspace!");
}
else if($status == "keyFound"){
	shareTelegram("Congratulations! ".$workername." found the key! Key is: ".$privatekey);
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

echo 'true';

?>
```

You must return `true` as a result from the API. The returned value is checked only when the key is found and new requests are sent until it receives a 'true' response from the API (Unlimited attempts).

### [**telegram_share**]

Share progress to Telegram

`true` Share progress to Telegram. 

`false` If false, it does not send notification. 

It sends a notification to Telegram when the private key is found. If you set "**telegram_share_eachkey**" to "true", it will send notification every time the scan is finished.

If your Telegram settings are correct, you will receive a notification that the worker has started working.

Suggestion: If you are on an untrusted computer, make the settings via the console and proceed without saving.

IMPORTANT: If the key is found and Telegram cannot transmit the key to you, it will make  unlimited attempts.

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

`true` When private key is found, <ins>**it only sends it via Telegram or API share**</ins>. Make sure your Telegram settings are correct. <ins>Otherwise, when the key is found, you will not be able to see it anywhere.</ins>

`false` When private key is found, The private key will be <ins>saved in a new text file</ins> and it <ins>appears on console screen</ins>. If Telegram share is active, notification will be sent.

---

### [**force_continue**] 

`true` If the private key is found, the scan will continue until it is finished. The related range is marked as "scanned" (like others). The scanned range will never be scanned by another user again. Telegram or api share feature must be active or insecure computer feature must be false. Because the scan will continue, the private key will not appear on the console screen.

`false` If the private key is found, the scanning process is terminated. No data is sent to the pool. It can be scanned again by another user after 12 hours (even with very low probability).

---


## Tips

1. If you are working on someone else's computer, set "untrusted_computer" to "true". However, you need to change either "api_share" or "telegram_share" to "true".
2. If possible, use "api_share" instead of "telegram_share". So you can save the private key in a file, send it via SMS or send it as mail.
3. Set "app_arguments" according to GPU model. This will give you higher performance.
4. I recommend using at least the "excludeIterated4" type in the scan type. It is very unlikely that such a private key will occur with hierarchically generated private keys.
5. If you are scanning with more than one video card, you can split the ranges. Even with two graphics cards, you can easily double your chances.
6. "DO NOT USE" closed-source bitcrack/vanitysearch app. Use only the one in the original repo or a open sourced bitcrack application. Make sure you're really working for yourself or someone else.

## When found?

`untrusted_computer=false` If the private key is found, it will appear on the console screen. Also, a new text file will be created in the folder where the application is run. (in the name of the target wallet address.)

`untrusted_computer=true` If the private key is found, it will send your Telegram channel/group only.

**[NOTE]** =>  `force_continue=true` and `untrusted_computer=true` and if Telegram and APi share feature is turned off, <ins>you will never get the key!</ins> Use the force_continue feature with caution and do not use it if you are not sure what it does.

# Create your Docker image

You can easily create your own image with the [Dockerfile](./Docker/Dockerfile) Docker image in the repo. With the Docker image, Bitcrackrandomiser, Bitcrack and VanitySearch will be pulled and built from the original github repos.

1 - Build docker file.

```bash
$ docker build -t bitcrackrandomiser .
```

2 - Tag the image (for push to hub). With your `{username}/{image_name}:{tag}.`

```bash
$ docker tag bitcrackrandomiser ilkercndk/bitcrackrandomiser:latest
```

3 - Push image to docker hub or use it local!

# General Information

1. This is <ins>not a shared pool</ins>.
2. If the private key is found, <ins>only you can see it</ins>. No one else can see!
3. If the private key is found, <ins>it is not shared</ins>.
4. Once a private key is scanned, it is not scanned again.
5. Your luck; 1 in 33 million.

# Donate
`1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw`


