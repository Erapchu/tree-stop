Кто дёргает ручку и попасть не может:

```bash
sudo grep "Failed password" /var/log/auth.log \
| awk '{print $(NF-3)}' \
| sort \
| uniq -c \
| sort -nr \
| head -20
```

Перезапуск:

```bash
sudo systemctl restart fail2ban
```

Забанить:

```bash
sudo fail2ban-client set sshd banip 1.2.3.4
```

Разбанить:

```bash
sudo fail2ban-client set sshd unbanip 45.148.10.240
```

Логи:

```bash
sudo journalctl -u fail2ban -n 100 --no-pager
```

Файл `/etc/fail2ban/jail.local`

```ini
[sshd]
enabled = true
port = ssh
filter = sshd
backend = systemd
maxretry = 5
findtime = 10m
bantime = 24h
```

Проверить настройки:

```bash
sudo fail2ban-client get sshd bantime
sudo fail2ban-client get sshd findtime
sudo fail2ban-client get sshd maxretry
```