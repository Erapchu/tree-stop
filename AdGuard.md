# AdGuard:

Find amnezia network interface (`amn0` is `172.29.172.1` for example). 

```bash
ip -br addr show
```

```bash
sudo mkdir -p /opt/adguard/{work,conf}
cd /opt/adguard
```

File:
/opt/adguard/docker-compose.yml

And forward address and port to AdGuard:

```yml
services:
  adguard:
    image: adguard/adguardhome:latest
    container_name: adguard
    restart: unless-stopped
    volumes:
      - ./work:/opt/adguardhome/work
      - ./conf:/opt/adguardhome/conf
    ports:
      # DNS only on localhost
      - "127.0.0.1:5353:53/tcp"
      - "127.0.0.1:5353:53/udp"

      # DNS via VPN-inteface (like amn0)
      - "172.29.172.1:53:53/udp"
      - "172.29.172.1:53:53/tcp"

      # UI locally (for yourself via SSH tunnel)
      - "127.0.0.1:3000:3000/tcp"

      # UI via WG (open from device when connected to AmneziaWG)
      - "172.29.172.1:3000:3000/tcp"
```

```bash
sudo docker compose up -d
```

On PC:
```
ssh -L 3000:127.0.0.1:3000 root@IP_VPS -i path_to_private_key
```

Open `http://127.0.0.1:3000` and configure.

Check AdGuard:

```bash
dig @127.0.0.1 -p 5353 doubleclick.net
dig @172.29.172.1 example.com
```

Set DNS server on client side to your new DNS like `172.29.172.1`.