resource "aws_db_subnet_group" "postgres" {
  name = "${var.project_name}-postgres-subnet-group"

  subnet_ids = [
    for subnet in aws_subnet.database : subnet.id
  ]

  tags = {
    Name = "${var.project_name}-postgres-subnet-group"
  }
}

resource "aws_db_instance" "postgres" {
  identifier = "${var.project_name}-postgres"

  engine         = "postgres"
  engine_version = "17"

  instance_class = "db.t4g.micro"

  allocated_storage     = 20
  max_allocated_storage = 100
  storage_type          = "gp3"
  storage_encrypted     = true

  db_name  = "ecommerce"
  username = var.database_username
  password = var.database_password

  port = 5432

  db_subnet_group_name   = aws_db_subnet_group.postgres.name
  vpc_security_group_ids = [aws_security_group.rds.id]

  publicly_accessible = false

  backup_retention_period = 7
  backup_window           = "03:00-04:00"

  maintenance_window = "sun:04:00-sun:05:00"

  auto_minor_version_upgrade = true

  deletion_protection = true

  skip_final_snapshot = false

  final_snapshot_identifier = "${var.project_name}-final-snapshot"

  multi_az = true

  monitoring_interval = 60

  performance_insights_enabled = true

  copy_tags_to_snapshot = true

  tags = {
    Name = "${var.project_name}-postgres"
  }
}

