docker run  -d --name {name} \
    -p 127.0.0.1:{port}:80 \
	-e "@ServiceOptions:Name={serviceName}" \
	-e "@ServiceOptions:GatewayProxyUri={gateway}" \
	-e "@ConnectionStrings:Redis={redis}" \
	{image}:{tag}