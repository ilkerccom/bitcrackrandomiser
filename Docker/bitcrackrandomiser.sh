#!/bin/sh

# Get GPU count
GPUCOUNTS=$(nvidia-smi -L |wc -l)
APPPATH="${BC_APP}"
APPARGS="${BC_APP_ARGS}"

# Set vanitysearch path and args
if [ "${BC_APP_TYPE}" = "vanitysearch" ]; then 
	APPPATH="/app/VanitySearch/./vanitysearch"
	if [ "${BC_APP_ARGS}" = "-b 896 -t 256 -p 256" ]; then
		APPARGS=""
	fi
fi

# Run bitcrackrandomiser
dotnet BitcrackRandomiser.dll \
	app_type=${BC_APP_TYPE} \
	app_path=${APPPATH} \
	app_arguments="${APPARGS}" \
	user_token=${BC_USERTOKEN} \
	wallet_address=${BC_WALLET} \
	gpu_count=${GPUCOUNTS} \
	gpu_index=${BC_GPUINDEX} \
	target_puzzle=${BC_PUZZLE} \
	scan_type=${BC_SCAN_TYPE} \
	scan_rewards=${BC_SCAN_REWARDS} \
	custom_range=${BC_CUSTOM_RANGE} \
	api_share=${BC_API_SHARE} \
	telegram_share=${BC_TELEGRAM_SHARE} \
	telegram_accesstoken=${BC_TELEGRAM_ACCESS_TOKEN} \
	telegram_chatid=${BC_TELEGRAM_CHAT_ID} \
	telegram_share_eachkey=${BC_TELEGRAM_SHARE_EACHKEY} \
	untrusted_computer=${BC_UNTRUSTED_COMPUTER} \
	force_continue=${BC_FORCE_CONTINUE} \
	private_pool=${BC_PRIVATEPOOL}