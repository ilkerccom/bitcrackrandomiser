#!/bin/sh

# Get GPU count
GPUCOUNTS=$(nvidia-smi -L |wc -l)

# Run bitcrackrandomiser
dotnet BitcrackRandomiser.dll \
	app_type=${BC_APP_TYPE} \
	app_path=${BC_APP} \
	app_arguments="${BC_APP_ARGS}" \
	user_token=${BC_USERTOKEN} \
	worker_name=${BC_WORKERNAME} \
	gpu_count=${GPUCOUNTS} \
	gpu_index=${BC_GPUINDEX} \
	gpu_seperated_range=${BC_GPUSEPERATEDRANGE} \
	target_puzzle=${BC_PUZZLE} \
	custom_range=${BC_CUSTOM_RANGE} \
	api_share=${BC_API_SHARE} \
	telegram_share=${BC_TELEGRAM_SHARE} \
	telegram_accesstoken=${BC_TELEGRAM_ACCESS_TOKEN} \
	telegram_chatid=${BC_TELEGRAM_CHAT_ID} \
	telegram_share_eachkey=${BC_TELEGRAM_SHARE_EACHKEY} \
	untrusted_computer=${BC_UNTRUSTED_COMPUTER} \
	force_continue=${BC_FORCE_CONTINUE} \
	cloud_search_mode=${BC_CLOUDSEARCHMODE}