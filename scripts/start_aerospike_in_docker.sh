#!/bin/bash
docker run -d --name aerospike -e "NAMESPACE=test" -p 3000-3002:3000-3002 aerospike:ce-6.2.0.7