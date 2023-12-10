<?php

declare(strict_types=1);

use App\Application\Actions\User\ListUsersAction;
use App\Application\Actions\User\ViewUserAction;
use Psr\Http\Message\ResponseInterface as Response;
use Psr\Http\Message\ServerRequestInterface as Request;
use Slim\App;
use Slim\Interfaces\RouteCollectorProxyInterface as Group;

return function (App $app) {
    $app->options('/{routes:.*}', function (Request $request, Response $response) {
        // CORS Pre-Flight OPTIONS Request Handler
        return $response;
    });

    $app->get('/', function (Request $request, Response $response) {
        $response->getBody()->write('Hello world!');
        return $response;
    });

    $app->group('/users', function (Group $group) {
        $group->get('', ListUsersAction::class);
        $group->get('/{id}', ViewUserAction::class);
    });


/****************** Table users ********************************/

// selectionner tout les user
$app->get('/GetAllUsers', function (Request $request, Response $response) {
    // $app->get('/API/GestionAbsenceAPI/getAllAbsence', function (Request $request, Response $response) {  //test via localhost
    $db = $this->get(PDO::class);
    $sth = $db->prepare("SELECT * FROM `users`");
    $sth->execute();
    $data = $sth->fetchAll(PDO::FETCH_ASSOC);
    $payload = json_encode($data);
    $response->getBody()->write($payload);
    return $response
        ->withHeader('Content-Type', 'application/json');
});

// ajouter un users
$app->post('/AddUsers', function (Request $request, Response $response) {
    $data = $request->getParsedBody();
    $db = $this->get(PDO::class);

    $sth = $db->prepare("INSERT INTO users (UserName, PasswordHash, Email, Phone, Birth, IV) 
                     VALUES (:UserName, :PasswordHash, :Email, :Phone, :Birth, :IV)");

    $sth->bindParam(":UserName", $data['UserName']);
    $sth->bindParam(":PasswordHash", $data['PasswordHash']);
    $sth->bindParam(":Email", $data['Email']);
    $sth->bindParam(":Phone", $data['Phone']);
    $sth->bindParam(":Birth", $data['Birth']);
    $sth->bindParam(":IV", $data['IV']);


    $sth->execute();

    $responseData = ["message" => "User entry created successfully"];
    $response->getBody()->write(json_encode($responseData));
    return $response
        ->withStatus(201)
        ->withHeader('Content-Type', 'application/json');
});


    $app->delete('/DelUsers/{Id}', function (Request $request, Response $response, array $args) {

        $id = $args['Id'];
        $db = $this->get(PDO::class);

        $sth = $db->prepare("DELETE FROM `users` WHERE `Id` = ?");

        $result = $sth->execute([$id]);

        // Vérifiez si la requête a bien été exécutée
        if ($result) {
            $response->getBody()->write(json_encode(["message" => "Utilisateur supprimé avec succès."]));
            return $response->withHeader('content-type', 'application/json')->withStatus(200);
        } else {
            $response->getBody()->write(json_encode(["message" => "Erreur lors de la suppression de l'utilisateur."]));
            return $response->withHeader('content-type', 'application/json')->withStatus(500);
        }
    });




// modifier users
$app->put('/EditUsers/{Id}', function (Request $request, Response $response, array $args) {
    $id = $args['Id'];
    $data = $request->getParsedBody();
    $db = $this->get(PDO::class);
    $sth = $db->prepare("UPDATE users SET UserName = :UserName, PasswordHash = :PasswordHash, Phone = :Phone, Email = :Email, Birth = :Birth, IV = :IV WHERE Id = :Id");
    $sth->bindParam(":Id", $id, PDO::PARAM_INT);
    $sth->bindParam(":UserName", $data['UserName']);
    $sth->bindParam(":PasswordHash", $data['PasswordHash']);
    $sth->bindParam(":Phone", $data['Phone']);
    $sth->bindParam(":Email", $data['Email']);
    $sth->bindParam(":Birth", $data['Birth']);
    $sth->bindParam(":IV", $data['IV']);
    $sth->execute();

    $responseData = ["message" => "User entry updated successfully"];
    $response->getBody()->write(json_encode($responseData));
    return $response
        ->withStatus(200)
        ->withHeader('Content-Type', 'application/json');
});



$app->get('/GetAllCredentials', function (Request $request, Response $response) {

    $db = $this->get(PDO::class);
    $sth = $db->prepare("SELECT * FROM `credentials`");
    $sth->execute();
    $data = $sth->fetchAll(PDO::FETCH_ASSOC);
    $payload = json_encode($data);
    $response->getBody()->write($payload);
    return $response
        ->withHeader('Content-Type', 'application/json');
});

    /****************** Table credentials ********************************/

// Ajouter un credentials
    $app->post('/AddCredentials', function (Request $request, Response $response) {
        $data = $request->getParsedBody();
        $db = $this->get(PDO::class);

        $sth = $db->prepare("INSERT INTO credentials (UserId, Application, UserNameApp, PasswordApp, Email, URL) 
                     VALUES (:UserId, :Application, :UserNameApp, :PasswordApp, :Email, :URL)");

        $sth->bindParam(":UserId", $data['UserId']);
        $sth->bindParam(":Application", $data['Application']);
        $sth->bindParam(":UserNameApp", $data['UserNameApp']);
        $sth->bindParam(":PasswordApp", $data['PasswordApp']);
        $sth->bindParam(":Email", $data['Email']);
        $sth->bindParam(":URL", $data['URL']);

        $sth->execute();

        $responseData = ["message" => "Credential entry created successfully"];
        $response->getBody()->write(json_encode($responseData));
        return $response
            ->withStatus(201)
            ->withHeader('Content-Type', 'application/json');
    });

// Supprimer un credentials par ID
    $app->delete('/DelCredentials/{Id}', function (Request $request, Response $response, array $args) {
        $id = $args['Id'];
        $db = $this->get(PDO::class);
        $sth = $db->prepare("DELETE FROM `credentials` WHERE `Id` = ?");
        $sth->execute([$id]);

        $responseData = ["message" => "Credential entry deleted successfully"];
        $response->getBody()->write(json_encode($responseData));
        return $response
            ->withStatus(200)
            ->withHeader('Content-Type', 'application/json');
    });

// Modifier credentials
    $app->put('/EditCredentials/{Id}', function (Request $request, Response $response, array $args) {
        $id = $args['Id'];
        $data = $request->getParsedBody();
        $db = $this->get(PDO::class);

        $sth = $db->prepare("UPDATE credentials SET UserId = :UserId, Application = :Application, UserNameApp = :UserNameApp, PasswordApp = :PasswordApp, Email = :Email, URL = :URL WHERE Id = :Id");

        $sth->bindParam(":Id", $id, PDO::PARAM_INT);
        $sth->bindParam(":UserId", $data['UserId']);
        $sth->bindParam(":Application", $data['Application']);
        $sth->bindParam(":UserNameApp", $data['UserNameApp']);
        $sth->bindParam(":PasswordApp", $data['PasswordApp']);
        $sth->bindParam(":Email", $data['Email']);
        $sth->bindParam(":URL", $data['URL']);
        $sth->execute();

        $responseData = ["message" => "Credential entry updated successfully"];
        $response->getBody()->write(json_encode($responseData));
        return $response
            ->withStatus(200)
            ->withHeader('Content-Type', 'application/json');
    });



};
