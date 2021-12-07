#!/bin/bash
docker run -d --name aerospike -e "NAMESPACE=test" -p 3000-3002:3000-3002 aerospike:ce-5.7.0.8