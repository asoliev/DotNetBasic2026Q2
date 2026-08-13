const cars = ["Saab", "Volvo", "BMW"];

const thirdCar = cars[2];
cars[0] = "Opel";
cars.pop();
cars.push("Audi");
cars.splice(1, 1);

console.log(thirdCar);
console.log(cars);