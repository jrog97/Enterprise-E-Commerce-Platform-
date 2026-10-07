resource "aws_secretsmanager_secret" "database" {
  name        = "${var.project_name}/database"
  description = "Database credentials for the Enterprise E-Commerce application"

  tags = {
    Name = "${var.project_name}-database-secret"
  }
}

resource "aws_secretsmanager_secret_version" "database" {
  secret_id = aws_secretsmanager_secret.database.id

  secret_string = jsonencode({
    username = var.database_username
    password = var.database_password
  })
}

resource "aws_secretsmanager_secret" "redis" {
  name        = "${var.project_name}/redis"
  description = "Redis/Valkey credentials for the Enterprise E-Commerce application"

  tags = {
    Name = "${var.project_name}-redis-secret"
  }
}

resource "aws_secretsmanager_secret_version" "redis" {
  secret_id = aws_secretsmanager_secret.redis.id

  secret_string = jsonencode({
    auth_token = var.redis_auth_token
  })
}

resource "aws_secretsmanager_secret" "jwt" {
  name        = "${var.project_name}/jwt"
  description = "JWT configuration for the Enterprise E-Commerce application"

  tags = {
    Name = "${var.project_name}-jwt-secret"
  }
}

resource "aws_secretsmanager_secret_version" "jwt" {
  secret_id = aws_secretsmanager_secret.jwt.id

  secret_string = jsonencode({
    issuer   = var.jwt_issuer
    audience = var.jwt_audience
    key      = var.jwt_key
  })
}