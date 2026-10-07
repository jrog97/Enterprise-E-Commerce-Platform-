resource "aws_cloudwatch_metric_alarm" "ecs_cpu_high" {
  alarm_name        = "${var.project_name}-ecs-cpu-high"
  alarm_description = "ECS service CPU utilization is too high."

  namespace   = "AWS/ECS"
  metric_name = "CPUUtilization"

  statistic = "Average"
  period    = 300

  evaluation_periods  = 2
  datapoints_to_alarm = 2

  threshold = 80

  comparison_operator = "GreaterThanThreshold"

  dimensions = {
    ClusterName = aws_ecs_cluster.main.name
    ServiceName = aws_ecs_service.api.name
  }

  treat_missing_data = "notBreaching"
}

resource "aws_cloudwatch_metric_alarm" "ecs_memory_high" {
  alarm_name        = "${var.project_name}-ecs-memory-high"
  alarm_description = "ECS service memory utilization is too high."

  namespace   = "AWS/ECS"
  metric_name = "MemoryUtilization"

  statistic = "Average"
  period    = 300

  evaluation_periods  = 2
  datapoints_to_alarm = 2

  threshold = 80

  comparison_operator = "GreaterThanThreshold"

  dimensions = {
    ClusterName = aws_ecs_cluster.main.name
    ServiceName = aws_ecs_service.api.name
  }

  treat_missing_data = "notBreaching"
}

resource "aws_cloudwatch_metric_alarm" "alb_5xx" {
  alarm_name        = "${var.project_name}-alb-5xx"
  alarm_description = "ALB is returning too many 5xx responses."

  namespace   = "AWS/ApplicationELB"
  metric_name = "HTTPCode_ELB_5XX_Count"

  statistic = "Sum"
  period    = 300

  evaluation_periods  = 2
  datapoints_to_alarm = 2

  threshold = 10

  comparison_operator = "GreaterThanThreshold"

  dimensions = {
    LoadBalancer = aws_lb.api.arn_suffix
  }

  treat_missing_data = "notBreaching"
}

resource "aws_cloudwatch_metric_alarm" "alb_unhealthy_targets" {
  alarm_name        = "${var.project_name}-alb-unhealthy-targets"
  alarm_description = "ALB has unhealthy ECS targets."

  namespace   = "AWS/ApplicationELB"
  metric_name = "UnHealthyHostCount"

  statistic = "Maximum"
  period    = 60

  evaluation_periods  = 2
  datapoints_to_alarm = 2

  threshold = 0

  comparison_operator = "GreaterThanThreshold"

  dimensions = {
    LoadBalancer = aws_lb.api.arn_suffix
    TargetGroup  = aws_lb_target_group.api.arn_suffix
  }

  treat_missing_data = "notBreaching"
}

resource "aws_cloudwatch_metric_alarm" "rds_cpu_high" {
  alarm_name        = "${var.project_name}-rds-cpu-high"
  alarm_description = "RDS CPU utilization is too high."

  namespace   = "AWS/RDS"
  metric_name = "CPUUtilization"

  statistic = "Average"
  period    = 300

  evaluation_periods  = 2
  datapoints_to_alarm = 2

  threshold = 80

  comparison_operator = "GreaterThanThreshold"

  dimensions = {
    DBInstanceIdentifier = aws_db_instance.postgres.id
  }

  treat_missing_data = "notBreaching"
}
resource "aws_cloudwatch_metric_alarm" "rds_storage_low" {
  alarm_name        = "${var.project_name}-rds-storage-low"
  alarm_description = "RDS free storage is getting low."

  namespace   = "AWS/RDS"
  metric_name = "FreeStorageSpace"

  statistic = "Minimum"
  period    = 300

  evaluation_periods  = 2
  datapoints_to_alarm = 2

  threshold = 5368709120

  comparison_operator = "LessThanThreshold"

  dimensions = {
    DBInstanceIdentifier = aws_db_instance.postgres.id
  }

  treat_missing_data = "notBreaching"
}