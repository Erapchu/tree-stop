https://github.com/TelegramMessenger/MTProxy

```bash
mkdir -p ~/mtproxy-official
cd ~/mtproxy-official

# Obtain a secret, used to connect to telegram servers
curl -s https://core.telegram.org/getProxySecret -o proxy-secret

# Obtain current telegram configuration. It can change (occasionally), so we encourage you to update it once per day.
curl -s https://core.telegram.org/getProxyConfig -o proxy-multi.conf

# Generate a secret to be used by users to connect to your proxy.
openssl rand -hex 16 > user-secret.txt
cat user-secret.txt
# OR
head -c 16 /dev/urandom | xxd -ps
```

docker-compose.yml:
```yml
services:
  mtproxy:
    image: telegrammessenger/proxy:latest
    container_name: mtproxy
    restart: unless-stopped
    network_mode: host
    volumes:
      - ./proxy-secret:/data/proxy-secret:ro
      - ./proxy-multi.conf:/data/proxy-multi.conf:ro
    command: >
      mtproto-proxy
      -u nobody
      -p 8888
      -H 443
      -S YOUR_USER_SECRET
      --aes-pwd /data/proxy-secret /data/proxy-multi.conf
      -M 1
```

... where:
- `nobody` is the username. `mtproto-proxy` calls `setuid()` to drop privileges.
- `443` is the port, used by clients to connect to the proxy.
- `8888` is the local port. You can use it to get statistics from `mtproto-proxy`. Like `wget localhost:8888/stats`. You can only get this stat via loopback.
- `<secret>` is the secret generated at step 3. Also you can set multiple secrets: `-S <secret1> -S <secret2>`.
- `proxy-secret` and `proxy-multi.conf` are obtained at steps 1 and 2.
- `1` is the number of workers. You can increase the number of workers, if you have a powerful server.

Generate the link with following schema: `tg://proxy?server=SERVER_NAME&port=PORT&secret=SECRET` (or let the official bot generate it for you).