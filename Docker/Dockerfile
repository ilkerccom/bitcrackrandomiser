FROM nvidia/cuda:12.0.0-devel-ubuntu20.04

# Variables
ENV BC_PUZZLE=66
ENV BC_USERTOKEN="0"
ENV BC_WALLET="1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw"
ENV BC_APP_TYPE="bitcrack"
ENV BC_APP="/app/BitCrack/bin/./cuBitCrack"
ENV BC_APP_ARGS="-b 896 -t 256 -p 256"
ENV BC_GPUCOUNT="1"
ENV BC_GPUINDEX="0"
ENV BC_SCAN_TYPE="includeDefeatedRanges"
ENV BC_SCAN_REWARDS="true"
ENV BC_CUSTOM_RANGE="none"
ENV BC_API_SHARE="none"
ENV BC_TELEGRAM_SHARE="false"
ENV BC_TELEGRAM_ACCESS_TOKEN="0"
ENV BC_TELEGRAM_CHAT_ID="0"
ENV BC_TELEGRAM_SHARE_EACHKEY="false"
ENV BC_UNTRUSTED_COMPUTER="false"
ENV BC_FORCE_CONTINUE="false"
ENV BC_PRIVATEPOOL="none"

# Clone and build bitcrack. For older graphics cards, you can lower the <COMPUTE_CAP> value.
WORKDIR /app
RUN apt-get update \
 && apt-get install git nano curl -y
RUN git clone https://github.com/brichard19/BitCrack
WORKDIR /app/BitCrack

# Build for AMD and OpenCL devices
#RUN make BUILD_OPENCL=1 COMPUTE_CAP=86
# Build for NVIDIA CUDA devices
RUN make BUILD_CUDA=1 COMPUTE_CAP=86

# Clone and build vanitysearch.
WORKDIR /app
RUN git clone https://github.com/ilkerccom/VanitySearch
WORKDIR /app/VanitySearch

# Build for CPU
#RUN make all
# Build for NVIDIA CUDA devices
RUN make gpu=1 CCAP=2.0 all

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

# Copy entrypoint
COPY bitcrackrandomiser.sh /app/bitcrackrandomiser
RUN chmod +x /app/bitcrackrandomiser/bitcrackrandomiser.sh

# Run bitcrackrandomiser
WORKDIR /app/bitcrackrandomiser
CMD ["/app/bitcrackrandomiser/bitcrackrandomiser.sh"]