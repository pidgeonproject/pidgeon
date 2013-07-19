all:
	./build.sh
debug:
	./build.sh --debug
clean:
	./build/clean.sh
install:
	./build/install.sh
uninstall:
	./build/uninstall.sh
