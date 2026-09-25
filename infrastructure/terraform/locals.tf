locals {
  availability_zones = [
    "us-east-2a",
    "us-east-2b"
  ]

  public_subnets = {
    "us-east-2a" = "172.31.1.0/24"
    "us-east-2b" = "172.31.2.0/24"
  }

  app_subnets = {
    "us-east-2a" = "172.31.11.0/24"
    "us-east-2b" = "172.31.12.0/24"
  }

  database_subnets = {
    "us-east-2a" = "172.31.21.0/24"
    "us-east-2b" = "172.31.22.0/24"
  }
}