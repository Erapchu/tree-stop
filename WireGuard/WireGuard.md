## WireGuard на Ubuntu

### Установка

```bash
sudo apt update
sudo apt install -y wireguard
```

### Генерация ключей

```bash
wg genkey | tee server_priv.key | wg pubkey > server_pub.key
wg genkey | tee client_priv.key | wg pubkey > client_pub.key
```

### Включить IP-форвардинг

```bash
sudo sysctl -w net.ipv4.ip_forward=1
echo 'net.ipv4.ip_forward=1' | sudo tee /etc/sysctl.d/99-wg.conf
sudo sysctl -p
```

### Конфиг сервера `/etc/wireguard/wg0.conf`

```ini
[Interface]
PrivateKey = <server_priv_key>
Address = 10.8.0.1/24
ListenPort = 51820
PostUp = iptables -A FORWARD -i wg0 -o eth0 -j ACCEPT; iptables -A FORWARD -i eth0 -o wg0 -m conntrack --ctstate RELATED,ESTABLISHED -j ACCEPT; iptables -t nat -A POSTROUTING -o eth0 -j MASQUERADE
PostDown = iptables -D FORWARD -i wg0 -o eth0 -j ACCEPT; iptables -D FORWARD -i eth0 -o wg0 -m conntrack --ctstate RELATED,ESTABLISHED -j ACCEPT; iptables -t nat -D POSTROUTING -o eth0 -j MASQUERADE

[Peer]
PublicKey = <client_pub_key>
AllowedIPs = 10.8.0.2/32
```

### Конфиг клиента

```ini
[Interface]
PrivateKey = <client_priv_key>
Address = 10.8.0.2/24
DNS = 1.1.1.1

[Peer]
PublicKey = <server_pub_key>
Endpoint = <server_ip>:51820
AllowedIPs = 0.0.0.0/0
PersistentKeepalive = 25
```

### Запуск

```bash
chmod 600 /etc/wireguard/wg0.conf

# Запустить и включить автозапуск
systemctl enable --now wg-quick@wg0
systemctl status wg-quick@wg0
wg show wg0
```

### Перезапуск:

```bash
systemctl restart wg-quick@wg0
```

### Остановить

```bash
systemctl stop wg-quick@wg0
systemctl disable wg-quick@wg0
```