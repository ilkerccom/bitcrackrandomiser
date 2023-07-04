FROM nvidia/cuda:12.0.0-devel-ubuntu20.04

# Variables
ENV BC_PUZZLE=66
ENV BC_WALLET="1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw"
ENV BC_APP="/app/BitCrack/bin/./cuBitCrack"
ENV BC_APP_ARGS="-b 896 -t 256 -p 256"
ENV BC_SCAN_TYPE="includeDefeatedRanges"
ENV BC_CUSTOM_RANGE="none"
ENV BC_TELEGRAM_SHARE="false"
ENV BC_TELEGRAM_ACCESS_TOKEN="0"
ENV BC_TELEGRAM_CHAT_ID="0"
ENV BC_UNTRUSTED_COMPUTER="false"

# Clone and build bitcrack
WORKDIR /app
RUN apt-get update \
 && apt-get install git nano curl -y
RUN git clone https://github.com/brichard19/BitCrack
WORKDIR /app/BitCrack
RUN make BUILD_CUDA=1 COMPUTE_CAP=86

# Install .NET Core
ENV DOTNET_SDK_VERSION 6.0.411
RUN curl -SL --output dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$DOTNET_SDK_VERSION/dotnet-sdk-$DOTNET_SDK_VERSION-linux-x64.tar.gz \
    && dotnet_sha512='dc8aa1f84ad97cf25a979bfc243c200b7a8e73b930b68d5eed782743d88aad823c32c267c83d7a19d3c4f910a8fae7f12d07ea5a35a1d3a97e13a8674d29037b' \
    && echo "$dotnet_sha512 dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
	DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

# Clone and build bitcrackrandomiser
WORKDIR /app/cloned
RUN git clone https://github.com/ilkerccom/bitcrackrandomiser.git
WORKDIR /app/cloned/bitcrackrandomiser/BitcrackRandomiser
RUN dotnet publish --configuration Release --output /app/bitcrackrandomiser

# Copy correct settings.txt file for this image
COPY settings.txt /app/bitcrackrandomiser

# Run bitcrackrandomiser
WORKDIR /app/bitcrackrandomiser
ENTRYPOINT dotnet BitcrackRandomiser.dll \
	app_path=${BC_APP} \
	app_arguments="${BC_APP_ARGS}" \
	wallet_address=${BC_WALLET} \
	target_puzzle=${BC_PUZZLE} \
	scan_type=${BC_SCAN_TYPE} \
	custom_range=${BC_CUSTOM_RANGE} \
	telegram_share=${BC_TELEGRAM_SHARE} \
	telegram_accesstoken=${BC_TELEGRAM_ACCESS_TOKEN} \ 
	telegram_chatid=${BC_TELEGRAM_CHAT_ID} \
	untrusted_computer=${BC_UNTRUSTED_COMPUTER}