# tree-stop

## 1 Pre-Configure

```bash
apt update
apt upgrade -y
ufw allow 80/tcp
reboot
```

## Configure (manually)

```bash
apt install -y curl wget socat cron
curl https://get.acme.sh | sh -s email=youremail@gmail.com
/root/.acme.sh/acme.sh --set-default-ca --server letsencrypt

bash <(curl -Ls https://raw.githubusercontent.com/mhsanaei/3x-ui/master/install.sh)
ufw allow xxx/tcp
```

### Check cron jobs (should be ~/.acme.sh)

```bash
crontab -l
```

## Configure (auto)

```bash
bash <(curl -Ls https://raw.githubusercontent.com/mhsanaei/3x-ui/master/install.sh)
ufw allow xxx/tcp
```

And set self-signed certificate.

On duckdns.org create new domain for you.

```bash
x-ui
18. SSL Certificate Management
1. Get SSL (Domain)
Port 80 (default)
reload for ACME - yes
```

# AdGuard:

```bash
sudo mkdir -p /opt/adguard/{work,conf}
cd /opt/adguard
```

### XRay

File:
/opt/adguard/docker-compose.yml

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
      # web-setup only locally too
      - "127.0.0.1:3000:3000/tcp"
```

```bash
sudo docker compose up -d
```

On PC:
```
ssh -L 3000:127.0.0.1:3000 root@IP_VPS
```

Open `http://127.0.0.1:3000` and configure.

On XRay:

```json
"dns": {
  "queryStrategy": "UseIPv4",
  "servers": [
    { "address": "127.0.0.1", "port": 5353 }
  ]
}
```

And then try. If it doesn't work set `routing.domainStrategy` not `AsIs`, but `IPIfNonMatch` or `IPOnDemand`.

Restart 3x-ui (xray).

Check AdGuard:

```bash
dig @127.0.0.1 -p 5353 doubleclick.net
```

### Amnezia

Check Amnezia interface:

```bash
ip -br addr show
```

Find amnezia network interface (`amn0` is `172.29.172.1` for example). And forward address and port to AdGuard:

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
ssh -L 3000:127.0.0.1:3000 root@IP_VPS
```

Just setup AdGuard as you like.

Check AdGuard:

```bash
dig @172.29.172.1 example.com
```

Set DNS server on client side to your new DNS like `172.29.172.1`.

# Proxy

Install any VPN on VPS as you like.
VPS with VPN it's a VPS2.
Remember the IP and port.

VPS1 -- proxy

VPS2 -- with VPN

On VPS1:

```bash
VPS2_IP="123.123.123.123"
AWG_PORT="54321"   # example, replace with AmneziaWG port for example
SSH_PORT="22"

sudo sysctl -w net.ipv4.ip_forward=1
echo 'net.ipv4.ip_forward = 1' | sudo tee /etc/sysctl.d/99-ipforward.conf >/dev/null
sudo sysctl -p /etc/sysctl.d/99-ipforward.conf

# NAT: inbound 54321 -> VPS2:54321
sudo iptables -t nat -A PREROUTING  -p udp --dport "$AWG_PORT" -j DNAT --to-destination "$VPS2_IP:$AWG_PORT"
sudo iptables -t nat -A POSTROUTING -p udp -d "$VPS2_IP" --dport "$AWG_PORT" -j MASQUERADE

# FORWARD only to VPS2:54321 + established
sudo iptables -A FORWARD -m conntrack --ctstate ESTABLISHED,RELATED -j ACCEPT
sudo iptables -A FORWARD -p udp -d "$VPS2_IP" --dport "$AWG_PORT" -m conntrack --ctstate NEW -j ACCEPT

# INPUT: lo, established, SSH
sudo iptables -A INPUT -i lo -j ACCEPT
sudo iptables -A INPUT -m conntrack --ctstate ESTABLISHED,RELATED -j ACCEPT
sudo iptables -A INPUT -p tcp --dport "$SSH_PORT" -j ACCEPT

# after checking close everything else
sudo iptables -P INPUT DROP
sudo iptables -P FORWARD DROP
sudo iptables -P OUTPUT ACCEPT

# Enable persistent rules
sudo apt install -y iptables-persistent
sudo netfilter-persistent save
```