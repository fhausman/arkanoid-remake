extends Node2D

func _on_AnimationPlayer_animation_finished(anim_name):
	if anim_name == "intro":
		get_node("AnimationPlayer").play("loop")
		
