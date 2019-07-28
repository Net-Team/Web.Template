read -p "请输入{image}:{tag}" imagetag
docker rmi $imagetag -f
docker build -t $imagetag .