build:
	dotnet build

test:
	dotnet test

docker-build:
	docker build -t ecommerce-api:latest .

docker-up:
	docker compose up -d --build

docker-down:
	docker compose down

docker-logs:
	docker compose logs -f api

health:
	curl http://localhost:8080/health