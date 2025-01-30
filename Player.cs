using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f; //Vitesse du joueur
	public const float JumpVelocity = -450.0f; //Vitesse des sauts (sur godot l'axe Y va du haut vers bas, d'où la valeur négative).
	public Vector2 ScreenSize; //Taille de l'écran
	
	public override void _Ready() //Fonction lancée une seule fois au démarrage.
	{
		//La fonction GetViewportRect() produit un rectangle de la taille de l'écran, la propriété .Size le transforme en un vecteur qui contient les dimensions x et y du rectangle.
		ScreenSize = GetViewportRect().Size;//On affecte au vecteur "ScreenSize" les dimensions de l'écran.
	}

	public override void _PhysicsProcess(double delta)//Fonction lancée à chaque frame.
	{
		//On crée un vecteur velocity auquel on affecte les valeurs de la propriété Velocity de MainCharacter qu'il hérite de la classe parente CharacterBody2D.
		Vector2 velocity = Velocity;

		//GRAVITE :
		if (!IsOnFloor())//Si n'est pas sur le sol
		{
			velocity += GetGravity() * (float)delta;//On ajoute à velocity la gravité (Y), X n'est pas affecté.
		}

		//DEPLACEMENT :
		//La fonction Input.GetVector prend 4 paramètres de type Input (gauche, droite, haut, bas) pour créer un vecteur contenant les coordonées x et y correspondante.
		//Les 4 paramètres (pour 4 directions) sont obligatoires donc on met "ui_down" et "ui_up" pour "haut" et "bas" même s'ils ne servent pas
		//On crée un vecteur "direction" qui contient les infos x et y entrées par l'utilisateur.
		Vector2 direction = Input.GetVector("move_left", "move_right", "ui_down", "ui_up");
		
		if (direction != Vector2.Zero)//Si direction n'est pas un vecteur nul
		{
			velocity.X = direction.X * Speed;//On affecte à velocity.X la valeur X de direction multiplié par la vitese du joueur.
		}
		else //(direction == Vector2.Zero) //Si direction est nul (joueur ne presse aucune touche).
		{
			//La fonction Mathf.MoveToward(debut, cible, décélération) rapproche progressivement "debut" de "cible" avec une vitesse de décéléation de "décélération".
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);//On rapproche velocity.X de 0 en utilisant la vitesse du joueur comme décélérateur.
		}

		
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");//On associe la variable animatedSprite2D au noeud du type AnimatedSprite2D (entre <>) nommé "AnimatedSprite2D" (entre ("")).
		animatedSprite2D.Play();//On lance les animations.
		
		//SAUT:
		if (Input.IsActionJustPressed("jump") && IsOnFloor())//Si "jump" (espace ou haut) est pressé et que le joueur et sur le sol
		{
			velocity.Y = JumpVelocity; //On affecte la vitesse de saut à l'axe Y de velocity.
		}
		
		//RETOURNEMENT DU SPRITE SELON LA DIRECTION:
		//La fonction FlipH de animatedSprite2D (H pour "Horizontal") retourne le sprite en miroir horizontalement.
		//Le sprite est par défaut orienté vers la droite
		if (velocity.X < 0)//Si velocity.X pointe vers la gauche
		{
			animatedSprite2D.FlipH = true;//On retourne le sprite. Le sprite reste retourné tant que le joueur ne pointe pas vers la droite.
		} else if (velocity.X > 0)//Si velocity.X pointe de nouveau vers la droite, on annule le retournement.
		{
			animatedSprite2D.FlipH = false;
		}
		//Faire comme ça permet que le sprite garde son orientation quand le joueur s'arrête, et pas qu'il s'oriente vers la droite des que "move_right" et lâché.
		
		//ANIMATIONS:
		if (velocity != Vector2.Zero)//si velocity n'est pas nul
		{
			animatedSprite2D.Animation = "walk";//jouer l'animation de marche "walk".
			if (velocity.Y < 0)//mais si velocity.Y va vers le haut
			{
				animatedSprite2D.Animation = "jump";//jouer animation de saut "jump".
			} 
			else if (velocity.Y > 0)//mais si velocity.Y va vers le bas.
			{
				animatedSprite2D.Animation = "fall";//Jouer animation de chute "fall".
			};
		}
		else//(velocity = Vector2.Zero)//si velocity est nul, le joueur est donc immobile.
		{
			animatedSprite2D.Animation = "idle";//Jouer animation d'immobilité "idle".
		};

		//LIMITE DE L'ECRAN:
		//la propriété Position de MainCharacter (qu'il hérite de la classe parente CharacterBody2D) est un vecteur qui définit la position du personnage.
		//On ajoute à Position le vecteur velocity pour définir la nouvelle position du personnage.
		Position += velocity * (float)delta;
		//La méthode Mathf.Clamp(nombre, limiteBasse, limiteHaute) limite la valeur que peux avoir "nombre" avec "limiteBasse" pour minimum et "limiteHaute" pour maximum.
		//On Définit position comme un nouveau Vector2, dont les valeur x et y sont limité entre 0 et la taille de l'écran (définit plus haut).
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		//On affecte velocity a la propriété Velocity de MainCharacter (qu'il hérite de la classe parente CharacterBody2D).
		Velocity = velocity;
		
		//La fonction MoveAndSlide() de CharacterBody2D calcule "automatiquement" les colisions, velocité...
		MoveAndSlide();
	}
}
