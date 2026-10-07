resource "aws_lb" "api" {
  name               = "${var.project_name}-alb"
  internal           = false
  load_balancer_type = "application"

  security_groups = [
    aws_security_group.alb.id
  ]

  subnets = [
    aws_subnet.public["us-east-2a"].id,
    aws_subnet.public["us-east-2b"].id
  ]

  enable_deletion_protection = false

  tags = {
    Name = "${var.project_name}-alb"
  }
}
resource "aws_lb_target_group" "api" {
  name        = "${var.project_name}-api-tg"
  port        = 8080
  protocol    = "HTTP"
  target_type = "ip"

  vpc_id = aws_vpc.main.id

  health_check {
    enabled = true

    path = "/health"

    protocol = "HTTP"

    port = "traffic-port"

    healthy_threshold   = 2
    unhealthy_threshold = 3

    timeout  = 5
    interval = 30

    matcher = "200"
  }

  tags = {
    Name = "${var.project_name}-api-target-group"
  }
}

resource "aws_lb_listener" "http" {
  load_balancer_arn = aws_lb.api.arn

  port     = 80
  protocol = "HTTP"

  default_action {
    type = "forward"

    forward {
      target_group {
        arn = aws_lb_target_group.api.arn
      }
    }
  }
}