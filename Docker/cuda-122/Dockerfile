#ilkercndk/bitcrackrandomiser:cuda-122
FROM nvidia/cuda:12.2.2-devel-ubuntu20.04

# Variables
ENV BC_PUZZLE=69
ENV BC_USERTOKEN="0"
ENV BC_WORKERNAME=""
ENV BC_APP_TYPE="vanitysearch"
ENV BC_APP="/app/VanitySearch/./vanitysearch"
ENV BC_APP_ARGS=""
ENV BC_GPUCOUNT="1"
ENV BC_GPUINDEX="0"
ENV BC_GPUSEPERATEDRANGE="true"
ENV BC_CUSTOM_RANGE="none"
ENV BC_API_SHARE="none"
ENV BC_TELEGRAM_SHARE="false"
ENV BC_TELEGRAM_ACCESS_TOKEN="0"
ENV BC_TELEGRAM_CHAT_ID="0"
ENV BC_TELEGRAM_SHARE_EACHKEY="false"
ENV BC_UNTRUSTED_COMPUTER="false"
ENV BC_FORCE_CONTINUE="false"
ENV BC_CLOUDSEARCHMODE="true"

# Install updates
WORKDIR /app
RUN apt-get update \
 && apt-get install git nano curl -y

# Clone and build vanitysearch.
WORKDIR /app
RUN git clone https://github.com/ilkerccom/VanitySearch-V2 /app/VanitySearch
WORKDIR /app/VanitySearch

# Build for NVIDIA CUDA devices
RUN make gpu=1 all

# Install .NET Core
ENV DOTNET_SDK_VERSION 8.0.406
RUN curl -SL --output dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$DOTNET_SDK_VERSION/dotnet-sdk-$DOTNET_SDK_VERSION-linux-x64.tar.gz \
    && dotnet_sha512='d6fdcfebd0df46959f7857cfb3beac7de6c8843515ece28b24802765fd9cfb6c7e9701b320134cb4907322937ab89cae914ddc21bf48b9b6313e9a9af5c1f24a' \
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

# Copy entrypoint
COPY bitcrackrandomiser.sh /app/bitcrackrandomiser
RUN chmod +x /app/bitcrackrandomiser/Apps/vanitysearch/./vanitysearch
RUN chmod +x /app/bitcrackrandomiser/bitcrackrandomiser.sh

# Its ready to run
WORKDIR /app/bitcrackrandomiser
#CMD ["/app/bitcrackrandomiser/bitcrackrandomiser.sh"]