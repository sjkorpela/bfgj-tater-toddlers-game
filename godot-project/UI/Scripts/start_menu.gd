extends Control


func _on_start_pressed() -> void:
	get_tree().change_scene_to_file("res://Assets/Scenes/test_scene_2.tscn")


func _on_settings_pressed() -> void:
	get_tree().change_scene_to_file("res://UI/Scenes/settings_menu.tscn")
