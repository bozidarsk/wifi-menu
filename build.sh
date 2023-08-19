# build: mono, gtk-sharp-3
# runtime: mono, gtk-sharp-3, gtk-layer-shell

mcs '-recurse:*.cs' -pkg:gtk-sharp-3.0 -out:wifi-menu
if [[ $? -eq 0 ]]; then
	chmod 755 ./wifi-menu
	sudo chown root:root ./wifi-menu
	sudo mv ./wifi-menu /usr/local/bin
else
	echo "Build failed."
	exit 1
fi
