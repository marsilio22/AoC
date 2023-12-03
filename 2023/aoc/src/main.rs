use std::fs;
use std::collections::HashMap;

fn main() {
    day1();
    day2();
    day3();
}

// todo move to mod
fn day1() {
    let contents = fs::read_to_string("./inputs/day1").expect("Should have read the file");
    let list = contents.split("\n");

    let nums = HashMap::from([
        ("1", "1"),
        ("2", "2"),
        ("3", "3"),
        ("4", "4"),
        ("5", "5"),
        ("6", "6"),
        ("7", "7"),
        ("8", "8"),
        ("9", "9"),
        ("one", "1"),
        ("two", "2"),
        ("three", "3"),
        ("four", "4"),
        ("five", "5"),
        ("six", "6"),
        ("seven", "7"),
        ("eight", "8"),
        ("nine", "9")
    ]);

    let mut results: [i32; 1000] = [0; 1000];
    let mut i = 0;

    for item in list {
        let mut earliest_first = i32::MAX;
        let mut latest_last = -1;

        let mut pair = ("", "");

        for num in nums.keys() {
            let first = item.find(num).map_or(i32::MAX, |g| {i32::try_from(g).expect("parse first usize to int")});
            let last = item.rfind(num).map_or(-1, |g| {i32::try_from(g).expect("parse last usize to int")});

            if first < earliest_first
            {
                earliest_first = first;
                pair.0 = nums[num];
            }

            if last > latest_last
            {
                latest_last = last;
                pair.1 = nums[num];
            }
        }

        let mut owned_string: String = "".to_owned();
        owned_string.push_str(pair.0);
        owned_string.push_str(pair.1);

        results[i] = owned_string.parse().expect("should've parsed");
        i = i+1;
    }

    let sum: i32 = results.iter().sum();

    println!("{}", sum);
}

fn day2() {
    let contents = fs::read_to_string("./inputs/day2").expect("Should have read the file");
    let input = contents.split("\n");

    let mut game = 1;
    let mut total = 0;
    let mut power_total = 0;

    let num_red = 12;
    let num_green = 13;
    let num_blue = 14;

    for entire_game in input {
        let pulls = entire_game.split("; ");
        
        let mut pulls_vec: Vec<&str> = pulls.collect();

        pulls_vec[0] = &pulls_vec[0].split(": ").last().expect("there should've been a colon in the first bit");

        let mut valid = true;
        let mut min_red = 0;
        let mut min_green = 0;
        let mut min_blue = 0;

        for pull in pulls_vec {
            let individual_ball_result = pull.split(", ");

            for set in individual_ball_result {
                let mut count_and_colour = set.split(" "); // why does `next`` need this to be mut?

                let ball_count = count_and_colour.next().expect("should have a count").parse::<i32>().expect("should've parsed");
                let ball_colour = count_and_colour.next().expect("should have a colour");

                match (ball_count, ball_colour) {
                    (a, "green") => {if a > num_green {valid = false;} if min_green < a { min_green = a;} },
                    (a, "red") => {if a > num_red {valid = false;} if min_red < a { min_red = a;} },
                    (a, "blue") => {if a > num_blue {valid = false;} if min_blue < a { min_blue = a;} },
                    _ => {}
                }
            }
        }
        
        if valid { total += game; }

        power_total += min_blue * min_green * min_red;
        
        game += 1;
    }

    println!("{}", total);
    println!("{}", power_total); 
}

fn day3() {
    let contents = fs::read_to_string("./inputs/day3").expect("Should have read the file");
    let rows = contents.split("\n");

    let mut map: HashMap<(i32, i32), (char, i32)> = HashMap::new();

    let mut j = 0;
    let mut i = 0;

    let numeric_characters = ['1', '2', '3', '4','5','6','7','8','9','0'];

    for row in rows {
        i=0;
        for character in row.chars() {
            if character == '.' { i += 1; continue; }
            
            let to_insert = if numeric_characters.contains(&character) { -1 } else { 0 };

            map.insert((i, j), (character, to_insert));
            i += 1;
        }

        let mut numbers_for_row: Vec<i32> = Vec::<i32>::new();
        for number in row.split(".")
        {
            if number == "" { continue }

            if number.parse::<i32>().is_ok()
            {
                numbers_for_row.push(number.parse::<i32>().expect("should've been ok"));
            }
        }

        let mut number_count = 0;

        for mut p in 0..i {
            if number_count >= numbers_for_row.len()
            {
                break;
            }

            if map.contains_key(&(p, j)) && map[&(p, j)].1 == -1 {
                while map.contains_key(&(p, j)) {
                    map.entry((p, j)).and_modify(|a| {(a.0, numbers_for_row[number_count]);});
                    p+=1;
                }
                number_count += 1;
            }
        }


        j += 1;
    }
    for thing in map {
        if thing.0.1 == 0 {
            println!("{:?}", thing);
        }
    }


}