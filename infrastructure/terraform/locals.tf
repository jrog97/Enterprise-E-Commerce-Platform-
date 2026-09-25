locals {
  availability_zones = [
    "us-east-1a",
    "us-east-1b"
  ]

  public_subnets = {
    "us-east-1a" = "10.0.1.0/24"
    "us-east-1b" = "10.0.2.0/24"
  }

  app_subnets = {
    "us-east-1a" = "10.0.11.0/24"
    "us-east-1b" = "10.0.12.0/24"
  }

  database_subnets = {
    "us-east-1a" = "10.0.21.0/24"
    "us-east-1b" = "10.0.22.0/24"
  }
}