
#build_image() {
#    SOURCE_HASH=$(find "$1" -type f -exec md5sum {} + | sort | md5sum | cut -d' ' -f1)
#    docker build --build-arg SOURCE_HASH="$SOURCE_HASH" -f "$1"/Dockerfile -t "$2" .
#}
#
#build_image "services/gateway" "wearecooking-gateway:latest"
#build_image "services/userinfo" "wearecooking-userinfo:latest"
#build_image "services/test" "wearecooking-rest-test:latest"

docker build -f services/test/Dockerfile . -t wearecooking-rest-test:latest
docker build -f services/ . -t wearecooking-userinfo:latest
docker build -f services/userinfo/Dockerfile . -t wearecooking-userinfo:latest
