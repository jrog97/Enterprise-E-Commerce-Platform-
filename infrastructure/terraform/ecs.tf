resource "aws_ecs_cluster" "main" {
  name = "${var.project_name}-cluster"

  setting {
    name  = "containerInsights"
    value = "enabled"
  }

  tags = {
    Name = "${var.project_name}-cluster"
  }
}
resource "aws_ecs_task_definition" "api" {
  family = "${var.project_name}-api"

  network_mode = "awsvpc"

  requires_compatibilities = [
    "FARGATE"
  ]

  cpu    = "512"
  memory = "1024"

  execution_role_arn = aws_iam_role.ecs_execution.arn
  task_role_arn      = aws_iam_role.ecs_task.arn

  container_definitions = jsonencode([
    {
      name  = "ecommerce-api"
      image = "${aws_ecr_repository.ecommerce_api.repository_url}:latest"

      essential = true

      portMappings = [
        {
          containerPort = 8080
          hostPort      = 8080
          protocol      = "tcp"
        }
      ]

      environment = [
        {
          name  = "ASPNETCORE_ENVIRONMENT"
          value = "Production"
        }
      ]

      secrets = [

        {
          name      = "DATABASE_USERNAME"
          valueFrom = "${aws_secretsmanager_secret.database.arn}:username::"
        },
        {
          name      = "DATABASE_PASSWORD"
          valueFrom = "${aws_secretsmanager_secret.database.arn}:password::"
        },
        {
          name      = "REDIS_AUTH_TOKEN"
          valueFrom = "${aws_secretsmanager_secret.redis.arn}:auth_token::"
        },
        {
          name      = "JWT_ISSUER"
          valueFrom = "${aws_secretsmanager_secret.jwt.arn}:issuer::"
        },
        {
          name      = "JWT_AUDIENCE"
          valueFrom = "${aws_secretsmanager_secret.jwt.arn}:audience::"
        },
        {
          name      = "JWT_KEY"
          valueFrom = "${aws_secretsmanager_secret.jwt.arn}:key::"
        }
      ]

      environment = [
        {
          name  = "ASPNETCORE_ENVIRONMENT"
          value = "Production"
        },
        {
          name  = "DATABASE_HOST"
          value = aws_db_instance.postgres.address
        },
        {
          name  = "DATABASE_PORT"
          value = tostring(aws_db_instance.postgres.port)
        },
        {
          name  = "DATABASE_NAME"
          value = aws_db_instance.postgres.db_name
        },
        {
          name  = "REDIS_HOST"
          value = aws_elasticache_replication_group.redis.primary_endpoint_address
        },
        {
          name  = "REDIS_PORT"
          value = tostring(aws_elasticache_replication_group.redis.port)
        }
      ]

      logConfiguration = {
        logDriver = "awslogs"

        options = {
          "awslogs-group"         = aws_cloudwatch_log_group.ecs.name
          "awslogs-region"        = var.aws_region
          "awslogs-stream-prefix" = "ecommerce-api"
        }
      }
    }
  ])

  tags = {
    Name = "${var.project_name}-api-task"
  }
}
resource "aws_ecs_service" "api" {
  name = "${var.project_name}-api"

  cluster = aws_ecs_cluster.main.id

  task_definition = aws_ecs_task_definition.api.arn

  desired_count = 2

  launch_type = "FARGATE"

  platform_version = "LATEST"

  deployment_circuit_breaker {
    enable   = true
    rollback = true
  }

  network_configuration {
    subnets = [
      for subnet in aws_subnet.app : subnet.id
    ]

    security_groups = [
      aws_security_group.ecs.id
    ]

    assign_public_ip = false
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.api.arn
    container_name   = "ecommerce-api"
    container_port   = 8080
  }

  deployment_minimum_healthy_percent = 50
  deployment_maximum_percent         = 200

  enable_ecs_managed_tags = true

  tags = {
    Name = "${var.project_name}-api-service"
  }

  lifecycle {
    ignore_changes = [
      task_definition
    ]
  }

}
