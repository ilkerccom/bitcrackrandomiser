# Run on Windows

Download and install .NET 8.0.x runtimes for Windows from [Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

1 - Go to [ilkerccom/bitcrackrandomiser/releases](https://github.com/ilkerccom/bitcrackrandomiser/releases) and download latest released "bitcrackrandomiser.zip" file.

2 - Unzip downloaded file.

![bitcrackrandomiser](https://i.ibb.co/5TWRSW9/ff.png)

3 - Edit `settings.txt` file from "bitcrackrandomiser" app folder.

a. Enter "user_token" value. Get your "user_token" from [btcpuzzle.info/user-center](https://btcpuzzle.info/user-center) website.

b. Save the file.

4 - Run the "**BitcrackRandomiser.exe**"

![bitcrackrandomiser](https://i.ibb.co/X542Gbw/bitcrack.png)

# Run on Windows as Docker Image

<ins>NOTE: ONLY FOR NVIDIA DEVICES!</ins> The relevant docker image only builds for "cuda devices". For AMD and OpenCL devices, use the "BUILD_OPENCL=1" argument in the dockerfile. (or go to Long Way section)

[Download and install Docker](https://docs.docker.com/desktop/install/windows-install/) app

There are two Docker images you can use for Bitcrackrandomiser.

## Manual Launch

`ilkercndk/bitcrackrandomiser:latest` will be used for manual launch.

1 - Open a new Command Prompt console and write below code.

```
docker run --gpus all -it ilkercndk/bitcrackrandomiser:latest
```

2 - Press enter and bingo!

![docker console](https://i.ibb.co/kckRTwJ/adaad.png)

3 - Edit settings file. You can use following code to edit on console.

```
$ nano settings.txt
```

Or you can edit "settings.txt" file from Docker app in currently active container.

![docker edit settings file](https://i.ibb.co/L8kZQsk/sdff.png)

4 - Enter following command end press ENTER.

```
$ dotnet BitcrackRandomiser.dll
```


# Run on CloudSearch

![CloudSearch by btcpuzzle.info](https://btcpuzzle.info/cloud-search.png)

Discover the easiest way to use the app and join the pool. Everything here is optimized for the application and the repository. Top up the balance and rent the instance at the hourly rates you want.

1 - Create an account on [BTCPuzzle.info](https://btcpuzzle.info/user-center) website and activate your CloudSearch account.

2 - Create template. The templates in Cloud Search are an area where you can create settings for “bitcrackrandomiser” and you can create as many templates as you want. Templates are used when creating instances.

![Templates on CloudSearch](https://btcpuzzle.info/faq/7.png)

3 - Top up balance (USD) using Polygon, Base, ETH and many other networks.

4 - Rent! To create a new instance, you first need a template that you can use. You can create new instances with default settings without using a template. Scroll down to the Cloud Search page and click on the “Search Instances” link. Select the template you want to use. Then, rent the instance by clicking the "Rent" button next to any instance you want to rent.

![Rent instance on CloudSearch](https://btcpuzzle.info/faq/9.gif)

You are now ready! You can read the questions you are wondering about on the [FAQ](https://btcpuzzle.info/faq). page.

# Run on Vast.ai

<ins>NOTE [1]: ONLY FOR NVIDIA DEVICES!</ins> The relevant docker image only builds for "cuda devices". For AMD and OpenCL devices, use the "BUILD_OPENCL=1" argument in the dockerfile. (or go to Long Way section)

<ins>NOTE [2]:</ins> When you use Vast.ai or similar, I recommend setting the value of "untrusted_computer" to "true" and turning on Telegram or Api share. Api share is always a better option instead of Telegram. If you don't, when the key is found it will be saved to the running instance. If you have not selected any sharing options, you will not receive a notification when the key is found.

Example docker options;

```
-e BC_UNTRUSTED_COMPUTER=true -e BC_API_SHARE=https://yourapiwebsite.com ...any other settings
```

For more information about API share, [go to related page](https://github.com/ilkerccom/bitcrackrandomiser?tab=readme-ov-file#api_share).

![security hash](https://gcdnb.pbrd.co/images/c3DzPemuL3Go.png?o=1)

Additionally, the "security hash" value (in your dashboard) that I underlined in red above is simple security information. This value changes when there is a change in the settings in which you run the application or when there is any code change in the application. With each new version (bitcrackrandomiser), new values are generated even if your settings remain the same.

Register [Vast.ai](https://cloud.vast.ai/?ref_id=69296) to rent GPU(s). 

## Easiest Way (Autorun)

Use custom docker image `ilkercndk/bitcrackrandomiser:autorun` from [dockerhub](https://hub.docker.com/r/ilkercndk/bitcrackrandomiser) in "Template Slot" and run any instance.

You can enter any settings you want in the "Docker options" field. "user_token" values are entered in this field.

You can create a template similar to the one above and rent the instance you want with this template. When you rent instance, bitcrackrandomiser will be run automatically according to the settings you enter. Make sure any of the Telegram or Api Share settings are active. It is also a useful setting to set the "untrusted_computer" value to "true" when using vast.ai. Thus, when the key is found, it is not saved anywhere in the running instance.

![vast ai console instance](https://i.ibb.co/565gP7L/vast2.png)

### Example

Using `ilkercndk/bitcrackrandomiser:latest` image.

#### 1- Docker options (Env. variables)

```bash
-e BC_USERTOKEN {YOUR_USERTOKEN_VALUE} -e BC_WORKERNAME=worker394
```

#### 2- Launch mode

[x] Interactive shell server, SSH

#### 3- On-start script

```
bash /app/bitcrackrandomiser/bitcrackrandomiser.sh
```

Save the template.

## Short Way (Manual)

Use custom docker image `ilkercndk/bitcrackrandomiser:latest` from [dockerhub](https://hub.docker.com/r/ilkercndk/bitcrackrandomiser) in "Template Slot" and run any instance.

Connect instance. Vast.ai doesnt support `--it` interactive arguments for docker image on SSH. You must go to main folder;

```
$ cd /app/bitcrackrandomiser
```

Then edit `settings.txt` file and run the app.

```bash
$ dotnet BitcrackRandomiser.dll
```

(Optional) **Auto-start bitcrackrandomiser** on instance starts; enter your parameters to on-start script on vast.ai. You can use all the variables in the <ins>settings.txt</ins> file.

```
$ cd /app/bitcrackrandomiser
$ dotnet BitcrackRandomiser.dll user_token=xxxx ...
```

Example docker create/run options

```
-e BC_USER_TOKEN=xxxx -e BC_UNTRUSTED_COMPUTER=true ...
```

## Long Way (Not recommended)

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
target_puzzle=68
app_type=bitcrack
app_path=/root/BitCrack/bin/./cuBitCrack
app_arguments=-b 896 -t 256 -p 256
user_token=xxxxxxxxxxxxxxxxxx
... other settings
```

Finally, run the app.

```
$ dotnet run
```

You can press "<ins>CTRL+B</ins>" and then "<ins>D</ins>" to leave terminal without closing app.
