.PHONY: build sample clean

build:
	@docker-compose up --build --no-start

sample: build
	@docker-compose up -d

unit-tests: build
	@docker-compose up --abort-on-container-exit --exit-code-from storage-service-unit-tests
	@docker-compose up --abort-on-container-exit --exit-code-from pixel-service-unit-tests

clean:
	@docker-compose down