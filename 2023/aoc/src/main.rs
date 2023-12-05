use std::fs;
use std::collections::HashMap;

fn main() {
    // day1();
    // day2();
    // day3();
    // day4();
    day5();
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

fn day4() {
    let contents = fs::read_to_string("./inputs/day4").expect("Should have read the file");
    let rows = contents.split("\n");
    let mut total_score = 0;

    let mut card_win_counts = HashMap::<i32, (i32, i32)>::new(); // tuple = (wins, count)
    let mut count = 1;

    for row in rows {
        let dump_first_bit = row.split(": ").last().expect("should've got last");
        let mut winning_and_actual = dump_first_bit.split(" | ");

        let winning = winning_and_actual.next().expect("should have first part");
        let actual = winning_and_actual.next().expect("should have second part");

        let winning_split = winning.split(" ").filter(|&x| !x.is_empty());
        let actual_split = actual.split(" ").filter(|&x| !x.is_empty());
        
        let winning_split_vec: Vec<&str> = winning_split.collect();
        let actual_vec: Vec<&str> = actual_split.collect();

        let mut score = 0;
        let mut win_count = 0;

        for winner in winning_split_vec {
            if actual_vec.contains(&winner)
            {
                win_count += 1;
                score = if score == 0 { 1 } else { score * 2 };
            }
        }

        total_score += score;
        card_win_counts.insert(count, (win_count, 1));
        count += 1;
    }

    let mut total_cards = 0;

    for i in 1..189 {
        if card_win_counts[&i].0 > 0 // wins > 0
        {
            let to_add = card_win_counts[&i].1;
            
            for j in 1..card_win_counts[&i].0 + 1 {
                *card_win_counts.get_mut(&(i+j)).unwrap() = (card_win_counts[&(i+j)].0, card_win_counts[&(i+j)].1 + to_add);
            }
        }
    }

    
    for card in card_win_counts.iter() {
        total_cards += card.1.1;
    }

    println!("{}", total_score);
    println!("{}", total_cards);

}


fn day5() {
    let contents = fs::read_to_string("./inputs/day5").expect("Should have read the file");
    let mut rows = contents.split("\n");

    let mut seeds: Vec<i64> = rows.next().expect("should be ok")
        .split(": ").last().expect("should still be ok")
        .split(" ").map(|x| x.parse::<i64>().expect("should have i64s")).collect();

    let mut seed_to_soil = Vec::<(i64, i64, i64)>::new();

    rows.next();
    rows.next(); // skip past the first title;

    loop {
        let nums = match rows.next() {
            Some(a) => a,
            None => break,
        };

        if nums.is_empty()// || nums.contains(":")
        {
            let seeds_copy = seeds.clone();
            
            // map the seeds now
            let mut cnt = 0;

            for seed in seeds_copy {
                for dest_src_len in seed_to_soil.iter() {
                    if seed > dest_src_len.1 && seed < dest_src_len.1 + dest_src_len.2 {
                        let new_val = seed - dest_src_len.1 + dest_src_len.0;
                        seeds[cnt] = new_val;
                    }
                }
                cnt += 1;
            }

            // clear the vec?
            seed_to_soil.clear();

            // skip the next title
            rows.next();
        }
        else
        {
            let ints:Vec<i64> = nums.split(" ").map(|x| x.parse::<i64>().expect("second time should have i64s")).collect();
            seed_to_soil.push((ints[0], ints[1], ints[2]));
        }
    }

    println!("{:?}", seeds);

}