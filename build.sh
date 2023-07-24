mcs '-recurse:*.cs' -pkg:gtk-sharp-3.0 -out:wifi-menu
chmod 755 ./wifi-menu
sudo chown root:root ./wifi-menu
sudo mv ./wifi-menu /usr/local/bin