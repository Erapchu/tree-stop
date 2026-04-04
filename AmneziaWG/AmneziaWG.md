```bash
# обновим пакеты, установим необходимые
apt update && apt install -y software-properties-common gnupg2 iptables

# Заголовки ядра -- нужны для сборки модуля AWG
apt install -y linux-headers-$(uname -r)

# PPA от Amnezia
add-apt-repository -y ppa:amnezia/ppa
apt update
apt install -y amneziawg amneziawg-tools

# Проверить что модуль загрузился
modprobe amneziawg
lsmod | grep amneziawg
awg --version

# Включить форвардинг
cat > /etc/sysctl.d/99-awg.conf << 'EOF'
net.ipv4.ip_forward = 1
net.ipv6.conf.all.forwarding = 1
EOF
sysctl -p /etc/sysctl.d/99-awg.conf

# Сгенерировать ключи сервера
mkdir -p /etc/amnezia/amneziawg
awg genkey | tee /etc/amnezia/amneziawg/server_private.key | awg pubkey > /etc/amnezia/amneziawg/server_public.key

# Этот ключ пойдёт в конфиги всех клиентов
cat /etc/amnezia/amneziawg/server_public.key

# Сначала узнать сетевой интерфейс
NET_IF=$(ip route show default | awk '{print $5; exit}')
echo $NET_IF    # обычно eth0, ens3 или что-то подобное
```

Пишем конфиг так:
```conf
# /etc/amnezia/amneziawg/awg0.conf
[Interface]
Address = 50.1.1.1/24
ListenPort = 443
PrivateKey = <server_private.key>
MTU = 1280

# Обфускацию подобрать самостоятельно или генератором, почитать доку
Jc = 53
Jmin = 313
Jmax = 1038
S1 = 54
S2 = 102
S3 = 31
S4 = 16
# диапазоны H1-H4 не должны пересекаться между собой
H1 = 471800590-471800690      # От 471,800,590 до 471,800,690
H2 = 1246894907-1246895000    # От 1,246,894,907 до 1,246,895,000
H3 = 923637689-923637690      # От 923,637,689 до 923,637,690
H4 = 1769581055-1869581055    # От 1,769,581,055 до 1,869,581,055
I1 = <b 0xc300000001><r 1><r 8><b 0x00><r 4><b 0x4001><r 24>
I2 = <b 0xc300000001><b 0x00><r 1><r 8><b 0x4001><r 24>
I3 = <b 0xe300000001><r 1><r 8><b 0x00><b 0x4001><r 16>

PostUp = iptables -A FORWARD -i awg0 -o ens3 -j ACCEPT; iptables -A FORWARD -i ens3 -o awg0 -m conntrack --ctstate RELATED,ESTABLISHED -j ACCEPT; iptables -t nat -A POSTROUTING -o ens3 -j MASQUERADE
PostDown = iptables -D FORWARD -i awg0 -o ens3 -j ACCEPT; iptables -D FORWARD -i ens3 -o awg0 -m conntrack --ctstate RELATED,ESTABLISHED -j ACCEPT; iptables -t nat -D POSTROUTING -o ens3 -j MASQUERADE

# Пиры добавлять ниже
```

```bash
chmod 600 /etc/amnezia/amneziawg/awg0.conf

# Запустить и включить автозапуск
systemctl enable --now awg-quick@awg0
systemctl status awg-quick@awg0
awg show awg0
```

Что проверять:
```bash
# Серверный форвардинг
sudo sysctl -w net.ipv4.ip_forward=1
echo 'net.ipv4.ip_forward=1' | sudo tee /etc/sysctl.d/99-awg.conf
sudo sysctl --system

iptables -L FORWARD -n -v --line-numbers
iptables -t nat -L POSTROUTING -n -v --line-numbers
```

Перезапуск:
```bash
systemctl restart awg-quick@awg0
```