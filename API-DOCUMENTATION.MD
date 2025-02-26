How to create your own client? If you see anything missing, please reply.

API Endpoint: https://api.btcpuzzle.info/

## `GET` /puzzle/{{PuzzleCode}}/range

Returns a new HEX value to be scanned from the pool.

Name |  Type | Required | Default Value | Info
--- | --- | --- |  --- | ---
PuzzleCode |  string ***(path)*** | yes | 68 | The puzzle number you want to scan. Possible values: 68, 69, 71 and 38 for test pool
UserToken|  string ***(*header)*** | yes | null | User token value of worker/user.
WorkerName|  string ***(*header)*** | no | null | Worker name. Min:1, max:15.
CustomRange|  string ***(*header)*** | no | null | The setting variables that you will use as the scan type are sent.

### Example request

https://api.btcpuzzle.info/puzzle/68/range

### Example Result

```
{
    "hex": "3AB8443",
    "puzzleCode": 68,
    "workloadStart": "0000000000",
    "workloadEnd": "3ffffffffff",
    "targetAddress": "1MVDYgVaSN6iKKEsbzRUAYFrYJadLYZvvZ",
    "proofOfWorkAddresses": [
        "1QLLeQ3Hwp9ai7L4KM65e7SxS12skkYUsA",
        "15mLWAX7Hno94fntWj6PEx5wCvZz2zAn8a",
        "1DazU9YxVrojvnTrcZrymyc6ZrDkvezJEA",
        "14Qada8GbVoKUXCtPHBgF7BPjdEhJWo1ph",
        "1PmdgtX2NzoSsSf2ANAnFrBzdftyW3CThY",
        "14it3UVGsK6JhsV2HPimD46VSpVcXipFwQ"
    ]
}
```

**HEX:** Specifies the HEX range to scan. Determine the starting point for Puzzle 66 by adding 10 zeros from ***"workloadStart"*** to the end of this HEX. Example; 3AB84430000000000

**workloadStart** | **workladEnd**: Total number of keys for the puzzle. In this example range must be; 3AB84430000000000:+3FFFFFFFFFF

**proofOfWorkAddresses:** The pool wants to make sure you're doing the scan correctly. Generates extra 6 private keys in the range 3AB844300000000000 to 3AB8443FFFFFFFFFF in this example. However, it returns the wallet addresses of the private keys generated in the pool for you to find. Marking will be done with the private key found.

You must scan the "x6 ProofValue" addresses simultaneously with the target address. You need to find these private keys and assign it to a variable. We'll use this later in the flag request.

```go
ExampleProofKey1 = 000000000000000000000000000000000000000000000003AB8443AB91BBAFE8; //for proofAddress1
ExampleProofKey2 = 000000000000000000000000000000000000000000000003AB84435AF1E51FD1; //for proofAddress2
ExampleProofKey3 = 000000000000000000000000000000000000000000000003AB8443DAFCC114AF; //for proofAddress3
ExampleProofKey4 = ....
ExampleProofKey5 = ....
ExampleProofKey6 = ....
```

The final proof key result is obtained as `SHA256(proofKey1+proofKey2+proofKey3+proofKey4+proofKey5+proofKey6)`

```
892a38381912bcce412030d7a403de8a92c0e56f452ba5a865f0bf728ff3fa87
```

There is rate limit: **60 request in 1 hour**

## `PUT` /puzzle/{{PuzzleCode}}/range

Flag/mark a scanned HEX value as "scanned" in the pool.

Name |  Type | Required | Default Value | Info
--- | --- | --- |  --- | ---
PuzzleCode|  string ***(*path)*** | yes | 66 | Which puzzle is being marked for? Possible values: 66,67,68 and 38 for test pool.
HEX|  string ***(*header)*** | yes | null | The HEX value to flag. Example: `3AB8443`
UserToken|  string ***(*header)*** | yes | null | The wallet address that made the flag. In short worker wallet address. Example: `1eosEvvesKV6C2ka4RDNZhmepm1TLFBtw.workername`
HashedProofKey|  string ***(*header)*** | yes | null | Private key of secondary wallet address sent as ProofValue. In 64 Bit HEX format. In this example: "Proof key" is `892a38381912bcce412030d7a403de8a92c0e56f452ba5a865f0bf728ff3fa87`
WorkerName|  string ***(*header)*** | no | null | Worker name.
GPUCount|  int ***(*header)*** | no | 1 | Count of GPU
IsForceContinue|  bool ***(*header)*** | no | false | Is force continue?

### Result

`RESPONSE -> OK (200)`

If the operation is successful, it returns with response code 200.


```
{ isSuccess: true }
```


`RESPONSE -> ERROR (400)`


```
{ isSuccess: false }
```

"ProofKey" is the private key of the wallet address you get with the GET method. "ProofValue" and "ProofKey" are updated on every request.

There is rate limit: **60 request in 1 hour**